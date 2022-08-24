using System.Numerics;
using SharpDX.Direct3D11;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace T3.Operators.Types.Id_883b3bce_65c8_417d_85fc_4fae259d537e
{
    public class UglySSAO2 : Instance<UglySSAO2>
    {

        [Output(Guid = "7a97d6f1-9125-4e05-86f0-c273ae000fcc")]
        public readonly Slot<T3.Core.Command> Output2 = new Slot<T3.Core.Command>();

        [Input(Guid = "ee7c9b76-9939-4a7a-9fac-e871119d1a5d")]
        public readonly InputSlot<System.Numerics.Vector2> NearFarRange = new InputSlot<System.Numerics.Vector2>();

        [Input(Guid = "9269ce22-3947-49dc-b955-ffd51907e441")]
        public readonly InputSlot<System.Numerics.Vector2> NearFarClip = new InputSlot<System.Numerics.Vector2>();

        [Input(Guid = "41fb7409-403b-4eff-be2a-71a89ae5472e")]
        public readonly InputSlot<System.Numerics.Vector4> Color = new InputSlot<System.Numerics.Vector4>();

        [Input(Guid = "a4c7038f-6869-4755-b902-f68b13e4e2ac")]
        public readonly InputSlot<System.Numerics.Vector2> BoostShadows = new InputSlot<System.Numerics.Vector2>();

        [Input(Guid = "750d99ae-3f7e-47bb-9a1e-a247e9f65e65")]
        public readonly InputSlot<float> Passes = new InputSlot<float>();

        [Input(Guid = "a1de128b-e922-46ed-9689-36f9648cdfd0")]
        public readonly InputSlot<float> Size = new InputSlot<float>();

        [Input(Guid = "c0bacd72-8c47-48fc-b03c-9385ddacb83e")]
        public readonly InputSlot<float> MixOriginal = new InputSlot<float>();

        [Input(Guid = "6ec1b455-0660-44f8-997a-cc35f7045b21")]
        public readonly InputSlot<float> MultiplyOriginal = new InputSlot<float>();

        [Input(Guid = "93f74912-f61f-40e0-92e7-2c0498558ec4")]
        public readonly InputSlot<System.Numerics.Vector2> NoiseOffset = new InputSlot<System.Numerics.Vector2>();

        [Input(Guid = "f7bc9799-c974-4e8a-8439-07fa7f03ee64")]
        public readonly InputSlot<SharpDX.Direct3D11.Texture2D> DepthBuffer = new InputSlot<SharpDX.Direct3D11.Texture2D>();

        [Input(Guid = "648c4a70-cc52-464f-b80a-ec441c2bdcdf")]
        public readonly InputSlot<SharpDX.Direct3D11.Texture2D> Texture2d = new InputSlot<SharpDX.Direct3D11.Texture2D>();

    }
}