using Unity.Entities;
using Unity.Mathematics;

namespace Enlighten.Evotico
{

    public struct CreatureMovementComponent : IComponentData
    {
        public CreatureMovementType currentMovementType;
        public CreatureMovementType desiredMovementType;
        public float currentSpeed;
        public float2 currentDirection;
        public float2 desiredDirection;
        public bool isMoving;
    }

}