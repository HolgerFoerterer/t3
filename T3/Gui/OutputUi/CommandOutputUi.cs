﻿using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System.Diagnostics;
using T3.Core;
using T3.Core.Operator;
using T3.Core.Operator.Slots;
using T3.Gui.Windows;
using Device = SharpDX.Direct3D11.Device;

namespace T3.Gui.OutputUi
{
    public class CommandOutputUi : OutputUi<Command>
    {
        protected override void Recompute(ISlot slot, EvaluationContext context)
        {
            // invalidate
            StartInvalidation(slot);

            // setup render target - TODO: this should not be done for all 'Command' outputs as most of them don't produce image content
            var resourceManager = ResourceManager.Instance();
            var device = resourceManager.Device;

            Size2 size = context.RequestedResolution;
            var wasRebuild = UpdateTextures(device, size, Format.R16G16B16A16_UNorm);
            var deviceContext = device.ImmediateContext;
            var prevViewports = deviceContext.Rasterizer.GetViewports<RawViewportF>();
            var prevTargets = deviceContext.OutputMerger.GetRenderTargets(1);
            deviceContext.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, size.Width, size.Height, 0.0f, 1.0f));
            deviceContext.OutputMerger.SetTargets(_colorBufferRtv);
            deviceContext.ClearRenderTargetView(_colorBufferRtv, new RawColor4(0.0f, 0.0f, 0.0f, 1.0f));

            // evaluate the op
            slot.Update(context);

            // restore prev setup
            deviceContext.Rasterizer.SetViewports(prevViewports);
            deviceContext.OutputMerger.SetTargets(prevTargets);

            // clean up ref counts for RTVs
            for (int i = 0; i < prevTargets.Length; i++)
            {
                prevTargets[i].Dispose();
            }
        }

        public override IOutputUi Clone()
        {
            return new CommandOutputUi()
                       {
                           OutputDefinition = OutputDefinition,
                           PosOnCanvas = PosOnCanvas,
                           Size = Size
                       };
        }

        protected override void DrawTypedValue(ISlot slot)
        {
            if (slot is Slot<Command> typedSlot)
            {
                ImageOutputCanvas.Current.DrawTexture(_colorBuffer);
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private bool UpdateTextures(Device device, Size2 size, Format format)
        {
            if (_colorBuffer != null
                && _colorBuffer.Description.Width == size.Width
                && _colorBuffer.Description.Height == size.Height
                && _colorBuffer.Description.Format == format)
                return false; // nothing changed

            _colorBuffer?.Dispose();
            _colorBufferSrv?.Dispose();
            _colorBufferRtv?.Dispose();

            var colorDesc = new Texture2DDescription()
                                {
                                    ArraySize = 1,
                                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                                    CpuAccessFlags = CpuAccessFlags.None,
                                    Format = format,
                                    Width = size.Width,
                                    Height = size.Height,
                                    MipLevels = 1,
                                    OptionFlags = ResourceOptionFlags.None,
                                    SampleDescription = new SampleDescription(1, 0),
                                    Usage = ResourceUsage.Default
                                };
            _colorBuffer = new Texture2D(device, colorDesc);
            _colorBufferSrv = new ShaderResourceView(device, _colorBuffer);
            _colorBufferRtv = new RenderTargetView(device, _colorBuffer);
            return true;
        }

        private Texture2D _colorBuffer;
        private ShaderResourceView _colorBufferSrv;

        private RenderTargetView _colorBufferRtv;
    }
}