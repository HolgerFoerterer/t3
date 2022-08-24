using System;
using SharpDX.Direct3D11;
using T3.Core.Logging;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace T3.Operators.Types.Id_719599b7_8348_45ac_8a14_059d01422d09 
{
    public class GetContextRenderTarget : Instance<GetContextRenderTarget>
    {
        [Output(Guid = "400EBED1-3EA8-4DBC-B76E-59404D283452")]
        public readonly Slot<Texture2D> Result = new();

        public GetContextRenderTarget()
        {
            Result.UpdateAction = Update;
        }

        private void Update(EvaluationContext context)
        {
            //Result.Value = context.RequestedResolution;
            // var v = Value.GetValue(context);
            // var mod = ModuloValue.GetValue(context);
            //
            // if (mod != 0)
            // {
            //     Result.Value = v - mod * (float)Math.Floor(v/mod);
            // }
            // else
            // {
            //     Log.Debug("Modulo caused division by zero", SymbolChildId);
            //     Result.Value = 0;
            // }
        }
        
        // [Input(Guid = "0496575f-df63-46bf-96ea-5d3d6c5af4ea")]
        // public readonly InputSlot<float> Value = new InputSlot<float>();
        //
        // [Input(Guid = "d25c47aa-0d2e-4f47-92ca-94d395722b63")]
        // public readonly InputSlot<float> ModuloValue = new InputSlot<float>();
    }
}
