using Enlighten.dotSprites;
using Unity.Entities;
using UnityEngine;

namespace Enlighten.Evotico
{

    [RequireComponent(typeof(SpriteAuthoring))]
    public class CreatureAuthoring : MonoBehaviour
    {
        public float AccelerationTime;
        public float MovementSpeed;
        public float RotationSpeed;
    }

    public class CreatureBaker : Baker<CreatureAuthoring>
    {
        public override void Bake(CreatureAuthoring authoring)
        {
            Entity creatureEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(creatureEntity, new CreatureMovementComponent());
            AddComponent(creatureEntity, new CreatureInfoComponent()
            {
                accelerationTime = authoring.AccelerationTime,
                movementSpeed = authoring.MovementSpeed,
                rotationSpeed = authoring.RotationSpeed
            });
        }
    }

}