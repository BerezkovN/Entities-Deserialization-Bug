using Unity.Entities;

namespace Enlighten.Evotico
{
    

    public struct CreatureInfoComponent : IComponentData
    {
        public float accelerationTime;
        public float runningTime;
        public float runningSpeed;
        public float movementSpeed;
        public float crouchSpeed;
        public float rotationTime;
    }

}