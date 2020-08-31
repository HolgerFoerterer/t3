﻿using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ManagedBass;
using Newtonsoft.Json;
using SharpDX;
using T3.Core.Logging;

namespace T3.Gui.UiHelpers
{
    public class SoundImageGenerator
    {
        public SoundImageGenerator(string filepath)
        {
            Filepath = filepath;
        }

        public string GenerateSoundSpectrumAndVolume()
        {
            var soundFilePath = Filepath;
            if (String.IsNullOrEmpty(soundFilePath) || !File.Exists(soundFilePath))
                return null;

            var imageFilePath = soundFilePath + ".waveform.png";
            if (File.Exists(imageFilePath))
            {
                Log.Debug($"Reusing sound image file: {imageFilePath}");
                return imageFilePath;
            }

            Log.Debug($"Generating {imageFilePath}...");

            Bass.Init(-1, 44100, 0, IntPtr.Zero);
            var stream = Bass.CreateStream(soundFilePath, 0, 0, BassFlags.Decode | BassFlags.Prescan);

            var streamLength = Bass.ChannelGetLength(stream);
            
            const double samplingResolution = 1.0 / 100;
            
            var sampleLength = Bass.ChannelSeconds2Bytes(stream, samplingResolution);
            var numSamples = streamLength / sampleLength;

            const int maxSamples = 16384;
            if (numSamples > maxSamples)
            {
                sampleLength = (long)( sampleLength * numSamples / (double)maxSamples) + 100;
                numSamples = streamLength / sampleLength;
                Log.Debug($"Limitting texture size to {numSamples} samples");
            }
            
            Bass.ChannelPlay(stream);

            var spectrumImage = new Bitmap((int)numSamples, ImageHeight);

            int a, b, r, g;
            var palette = new System.Drawing.Color[PaletteSize];
            int palettePos;

            for (palettePos = 0; palettePos < PaletteSize; ++palettePos)
            {
                a = 255;
                if (palettePos < PaletteSize * 0.666f)
                    a = (int)(palettePos * 255 / (PaletteSize * 0.666f));

                b = 0;
                if (palettePos < PaletteSize * 0.333f)
                    b = palettePos;
                else if (palettePos < PaletteSize * 0.666f)
                    b = -palettePos + 510;

                r = 0;
                if (palettePos > PaletteSize * 0.666f)
                    r = 255;
                else if (palettePos > PaletteSize * 0.333f)
                    r = palettePos - 255;

                g = 0;
                if (palettePos > PaletteSize * 0.666f)
                    g = palettePos - 510;

                palette[palettePos] = System.Drawing.Color.FromArgb(a, r, g, b);
            }

            foreach (var region in _regions)
            {
                region.Levels = new float[numSamples];
            }

            var f = (float)(SpectrumLength / Math.Log(ImageHeight + 1));
            var f2 = (float)((PaletteSize - 1) / Math.Log(MaxIntensity + 1));
            //var f3 = (float)((ImageHeight - 1) / Math.Log(32768.0f + 1));

            for (var sampleIndex = 0; sampleIndex < numSamples; ++sampleIndex)
            {
                Bass.ChannelSetPosition(stream, sampleIndex * sampleLength);
                Bass.ChannelGetData(stream, _fftBuffer, (int)DataFlags.FFT2048);

                for (var rowIndex = 0; rowIndex < ImageHeight; ++rowIndex)
                {
                    var j = (int)(f * Math.Log(rowIndex + 1));
                    var pj = (int)(rowIndex > 0 ? f * Math.Log(rowIndex - 1 + 1) : j);
                    var nj = (int)(rowIndex < ImageHeight - 1 ? f * Math.Log(rowIndex + 1 + 1) : j);
                    var intensity = 125.0f * _fftBuffer[SpectrumLength - pj - 1] +
                                    750.0f * _fftBuffer[SpectrumLength - j - 1] +
                                    125.0f * _fftBuffer[SpectrumLength - nj - 1];
                    intensity = Math.Min(MaxIntensity, intensity);
                    intensity = Math.Max(0.0f, intensity);

                    palettePos = (int)(f2 * Math.Log(intensity + 1));
                    spectrumImage.SetPixel(sampleIndex, rowIndex, palette[palettePos]);
                }

                if (sampleIndex % 1000 == 0)
                {
                    Log.Debug($"   computing sound image {100 * sampleIndex / numSamples}% complete");
                }

                foreach (var region in _regions)
                {
                    region.ComputeUpLevelForCurrentFft(sampleIndex, ref _fftBuffer);
                }
            }

            foreach (var region in _regions)
            {
                region.SaveToFile(soundFilePath);
            }

            spectrumImage.Save(imageFilePath);
            Bass.ChannelStop(stream);
            Bass.StreamFree(stream);

            return imageFilePath;
        }

        private class FftRegion
        {
            public string Title;
            public float[] Levels;
            public float LowerLimit;
            public float UpperLimit;

            public void ComputeUpLevelForCurrentFft(int index, ref float[] fftBuffer)
            {
                var level = 0f;


                var startIndex = (int)MathUtil.Lerp(0, SpectrumLength, MathUtil.Clamp(this.LowerLimit, 0, 1));
                var endIndex = (int)MathUtil.Lerp(0, SpectrumLength, MathUtil.Lerp(this.UpperLimit, 0, 1));

                for (int i = startIndex; i < endIndex; i++)
                {
                    level += fftBuffer[i];
                }

                Levels[index] = level;
            }

            public void SaveToFile(string basePath)
            {
                using (var sw = new StreamWriter(basePath + "." + Title + ".json"))
                {
                    sw.Write(JsonConvert.SerializeObject(Levels, Formatting.Indented));
                }
            }
        }


        private readonly FftRegion[] _regions = {
                                                        new FftRegion() { Title = "levels", LowerLimit = 0f, UpperLimit = 1f },
                                                        new FftRegion() { Title = "highlevels", LowerLimit = 0.3f, UpperLimit = 1f },
                                                        new FftRegion() { Title = "midlevels", LowerLimit = 0.06f, UpperLimit = 0.3f },
                                                        new FftRegion() { Title = "lowlevels", LowerLimit = 0.0f, UpperLimit = 0.02f },
                                                    };

        private const int SpectrumLength = 1024;
        private const int ImageHeight = 256;
        private const float MaxIntensity = 500;
        private const int ColorSteps = 255;
        private const int PaletteSize = 3 * ColorSteps;
        public readonly string Filepath;

        private float[] _fftBuffer = new float[SpectrumLength];
        
    }
}