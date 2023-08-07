using Unity.Entities;

namespace Enlighten.Evotico
{

    public struct CreatureInfoComponent : IComponentData
    {
        public float accelerationTime;
        public float movementSpeed;
        public float rotationSpeed;
    }

}