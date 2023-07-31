using Unity.Entities;
using Unity.Mathematics;

namespace Enlighten.Evotico
{

    public struct CreatureMovementComponent : IComponentData
    {
        public float2 direction;
        public bool isMoving;
    }

}