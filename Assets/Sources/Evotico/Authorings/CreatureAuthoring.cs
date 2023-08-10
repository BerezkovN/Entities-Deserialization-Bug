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
        public float AccelerationTime;
        public float RunningTime;
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
                movementType = CreatureMovementType.STOP,
                currentDirection = new float2(-math.sin(rotation), math.cos(rotation))
            });
            AddComponent(creatureEntity, new CreatureInfoComponent()
            {
                accelerationTime = authoring.AccelerationTime,
                runningSpeed  = authoring.RunningSpeed,
                runningTime   = authoring.CrouchSpeed, 
                movementSpeed = authoring.MovementSpeed,
                crouchSpeed   = authoring.CrouchSpeed,
                rotationTime = authoring.RotationTime
            });
        }
    }

}