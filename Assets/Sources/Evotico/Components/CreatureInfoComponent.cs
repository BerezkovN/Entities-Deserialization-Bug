using Unity.Entities;

namespace Enlighten.Evotico
{
    

    public struct CreatureInfoComponent : IComponentData
    {
        public float skiddingAngle;
        
        public float accelerationTime;
        public float stoppingTime;
        
        public float runningTime;
        public float runningSpeed;
        
        public float movementSpeed;
        public float crouchSpeed;
        
        public float rotationTime;
    }

}