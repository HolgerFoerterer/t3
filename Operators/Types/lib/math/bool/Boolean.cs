using System.Numerics;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace T3.Operators.Types.Id_ed0f5188_8888_453e_8db4_20d87d18e9f4
{
    public class Boolean : Instance<Boolean>
    {
        [Output(Guid = "97A91F72-1E40-412C-911E-70B142E16925")]
        public readonly Slot<bool> Result = new();

        public Boolean()
        {
            Result.UpdateAction = Update;
        }

        private void Update(EvaluationContext context)
        {
            Result.Value = BoolValue.GetValue(context);
        }
        
        [Input(Guid = "E7C1F0AF-DA6D-4E33-AC86-7DC96BFE7EB3")]
        public readonly InputSlot<bool> BoolValue = new();
        
        [Input(Guid = "49b3fa80-4027-4297-82cd-10a36882ed9d")]
        public readonly InputSlot<Vector4> ColorInGraph = new();
    }
}
