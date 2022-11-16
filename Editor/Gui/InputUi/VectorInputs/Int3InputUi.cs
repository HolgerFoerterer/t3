﻿using System.Linq;
using Editor.Gui.Interaction;
using SharpDX;
using T3.Core.Animation;
using T3.Core.Operator;
using T3.Core.Operator.Slots;
using Editor.Gui.InputUi;
using T3.Editor.Gui.InputUi;

namespace Editor.Gui.InputUi.VectorInputs
{
    public class Int3InputUi : IntVectorInputValueUi<Int3>
    {
        public override bool IsAnimatable => true;

        public Int3InputUi() : base(3)
        {
        }

        public override IInputUi Clone()
        {
            return CloneWithType<Int3InputUi>();
        }

        protected override InputEditStateFlags DrawEditControl(string name, ref Int3 int3Value)
        {
            IntComponents[0] = int3Value.X;
            IntComponents[1] = int3Value.Y;
            IntComponents[2] = int3Value.Z;

            var inputEditState = VectorValueEdit.Draw(IntComponents, Min, Max, Scale, Clamp);
            int3Value = new Int3(IntComponents[0], IntComponents[1], IntComponents[2]);

            return inputEditState;
        }


        public override void ApplyValueToAnimation(IInputSlot inputSlot, InputValue inputValue, Animator animator, double time)
        {
            if (inputValue is not InputValue<Int3> typedInputValue)
                return;

            var curves = animator.GetCurvesForInput(inputSlot).ToArray();
            IntComponents[0] = typedInputValue.Value.X;
            IntComponents[1] = typedInputValue.Value.Y;
            IntComponents[2] = typedInputValue.Value.Z;
            Curve.UpdateCurveValues(curves, time, IntComponents);
        }
    }
}