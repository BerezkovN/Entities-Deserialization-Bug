using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Enlighten.Evotico
{

    public partial struct CreatureMovementSystem : ISystem
    {
        private EntityQuery creatureQuery;

        public void OnCreate(ref SystemState state)
        {
            creatureQuery = state.GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                        {
                            ComponentType.ReadWrite<CreatureMovementComponent>(),
                            ComponentType.ReadOnly<CreatureInfoComponent>(),
                            ComponentType.ReadOnly<LocalTransform>()
                        }
                }
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            CreatureMovementJob job = new CreatureMovementJob()
            {
                deltaTime = state.World.Time.DeltaTime
            };
            job.ScheduleParallel(creatureQuery, state.Dependency).Complete();
        }


        [BurstCompile]
        private partial struct CreatureMovementJob : IJobEntity
        {
            private static readonly float DEFAULT_ACCELERATION_TIME = 10f; 
            public float deltaTime;
            
            public void Execute(ref LocalTransform localTransform, ref CreatureMovementComponent creatureMovement, in CreatureInfoComponent creatureInfo)
            {
                float currentDesiredSpeed = 0;

                switch (creatureMovement.movementType)
                {
                    case CreatureMovementType.STOP:

                        if (creatureMovement.currentSpeed == 0) {
                            return;
                        }
                        currentDesiredSpeed = 0;
                        break;
                    
                    case CreatureMovementType.MOVE:
                        currentDesiredSpeed = creatureInfo.movementSpeed;
                        break;
                }

                creatureMovement.currentSpeed += (currentDesiredSpeed / DEFAULT_ACCELERATION_TIME) * deltaTime;
                creatureMovement.currentSpeed = math.min(creatureMovement.currentSpeed, currentDesiredSpeed);
                
                var movementVector = creatureMovement.currentDirection * creatureMovement.currentSpeed;

                // float angle = -(float)math.atan2(creatureMovement.currentSpeed.x, creatureMovement.currentSpeed.y);
                // localTransform.Rotation.value = quaternion.RotateZ(angle).value;

                //this.clampMagnitude(ref movementVector, currentDesiredSpeed);
                float2 newPosition = localTransform.Position.xy + movementVector * deltaTime;
                
                localTransform.Position.xy = newPosition;
            }

            private void clampMagnitude(ref float2 vector, float maxLength)
            {
                float sqrMagnitude = math.lengthsq(vector);
                if ((double) sqrMagnitude <= (double) maxLength * (double) maxLength)
                    return;
                
                float num1 = (float) math.sqrt((double) sqrMagnitude);
                float num2 = vector.x / num1;
                float num3 = vector.y / num1;
                vector.x = num2 * maxLength;
                vector.y = num3 * maxLength;
            }
        }
    }
}