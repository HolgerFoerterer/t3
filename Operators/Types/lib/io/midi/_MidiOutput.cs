using System;
using System.Collections.Generic;
using NAudio.Midi;
using Operators.Utils;
using T3.Core.Logging;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Interfaces;
using T3.Core.Operator.Slots;
using T3.Core.Utils;

namespace T3.Operators.Types.Id_f9f4281b_92ee_430d_a930_6b588a5cb9a9 
{
    public class _MidiOutput : Instance<_MidiOutput>, ICustomDropdownHolder, IStatusProvider
    {
        [Output(Guid = "8ccaa82b-5acf-4556-b2e4-2bd1c13ce929")]
        public readonly Slot<float> Result = new Slot<float>();

        public _MidiOutput()
        {
            Result.UpdateAction = Update;
        }

        private void Update(EvaluationContext context)
        {
            var deviceName = Device.GetValue(context);
            var foundDevice = false;
            foreach (var (m, device) in MidiInConnectionManager._midiOutsWithDevices)
            {
                if (device.ProductName != deviceName)
                    continue;
                
                var channel = Channel.GetValue(context).Clamp(1,16);
                var controllerIndex = Controller.GetValue(context).Clamp(0, 127);
                var velocity = Velocity.GetValue(context).Clamp(0, 127);
                try
                {
                    //var midiEvent = new ControlChangeEvent(0, channel, (MidiController)controllerIndex, velocity);
                    var midiEvent = new NoteOnEvent(0, channel, controllerIndex, velocity, 50);
                    m.Send(midiEvent.GetAsShortMessage());
                    Log.Debug("Sending MidiTo " + device.Manufacturer + " " + device.ProductName, this);
                    foundDevice = true;
                }
                catch (Exception e)
                {
                    Log.Warning("Failed to send midi:" + e.Message, this);
                }
            }

            _lastErrorMessage = !foundDevice ? $"Can't find MidiDevice {deviceName}" : null;
        }
        
        [Input(Guid = "53b4de88-59e9-456e-be2c-d1825e2ffd6f")]
        public readonly InputSlot<float> Value = new InputSlot<float>();
        
        [Input(Guid = "A7E1EAC2-5602-4C40-8519-19CA53763C76")]
        public readonly InputSlot<int> Channel = new ();

        [Input(Guid = "ADEA6968-35EF-436A-BC2D-D9433B623DF6")]
        public readonly InputSlot<string> Device = new ();
        
        [Input(Guid = "0FFF2CE2-DEFA-442C-A089-4B12E7D71620")]
        public readonly InputSlot<int> Controller = new ();

        [Input(Guid = "3ECF2FCF-0F26-4593-804E-A5EF99057D9E")]
        public readonly InputSlot<int> Velocity = new ();
        
        [Input(Guid = "55aaed23-e2b9-4f17-86e2-ea8ffb347c0a")]
        public readonly InputSlot<float> ModuloValue = new InputSlot<float>();

        public string GetValueForInput(Guid inputId)
        {
            return Device.Value;
        }

        public IEnumerable<string> GetOptionsForInput(Guid inputId)
        {
            if (inputId != Device.Id)
            {
                yield return "undefined";
                yield break;
            }
            
            foreach (var device in MidiInConnectionManager._midiOutsWithDevices.Values)
            {
                yield return device.ProductName;
            }
        }

        public void HandleResultForInput(Guid inputId, string result)
        {
            Log.Debug($"Got {result}", this);
            Device.SetTypedInputValue(result);
        }

        public IStatusProvider.StatusLevel GetStatusLevel()
        {
            return string.IsNullOrEmpty(_lastErrorMessage) ? IStatusProvider.StatusLevel.Success : IStatusProvider.StatusLevel.Error;
        }

        public string GetStatusMessage()
        {
            return _lastErrorMessage;
        }

        private string _lastErrorMessage;
    }
}