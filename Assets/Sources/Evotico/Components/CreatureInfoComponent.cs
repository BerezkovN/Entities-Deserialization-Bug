using Unity.Entities;

namespace Enlighten.Evotico
{

    public enum CreatureMovementType
    {
        CROUCH,
        NORMAL,
        SPRINT
    }

    public struct CreatureInfoComponent : IComponentData
    {
        public float accelerationTime;
        public float movementSpeed;
        public float rotationSpeed;
    }

}