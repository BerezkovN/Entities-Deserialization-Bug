using Enlighten.dotSprites;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Enlighten.Evotico
{

    [RequireComponent(typeof(SpriteAuthoring))]
    public class CreatureAuthoring : MonoBehaviour
    {
        public float SkiddingAngle;
        
        public float AccelerationTime;
        public float StoppingTime;
        
        public float RunningSpeed;
        public float MovementSpeed;
        public float CrouchSpeed;
        
        public float RotationTime;
    }

    public class CreatureBaker : Baker<CreatureAuthoring>
    {
        public override void Bake(CreatureAuthoring authoring)
        {
            Entity creatureEntity = GetEntity(TransformUsageFlags.Dynamic);

            float rotation = authoring.transform.rotation.eulerAngles.z;
            AddComponent(creatureEntity, new CreatureMovementComponent()
            {
                currentMovementType = CreatureMovementType.STAY,
                currentDirection = new float2(-math.sin(rotation), math.cos(rotation))
            });
            AddComponent(creatureEntity, new CreatureInfoComponent()
            {
                skiddingAngle = math.radians(authoring.SkiddingAngle),
                accelerationTime = authoring.AccelerationTime,
                stoppingTime  = authoring.StoppingTime, 
                runningSpeed  = authoring.RunningSpeed,
                movementSpeed = authoring.MovementSpeed,
                crouchSpeed   = authoring.CrouchSpeed,
                rotationTime = authoring.RotationTime
            });
        }
    }

}