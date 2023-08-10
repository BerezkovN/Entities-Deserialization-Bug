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

            public float deltaTime;
            
            public void Execute(ref LocalTransform localTransform, ref CreatureMovementComponent creatureMovement, in CreatureInfoComponent creatureInfo)
            {
                float currentDesiredSpeed = getDesiredSpeed(ref creatureMovement.desiredMovementType, in creatureInfo);

                bool isAccelerating = (currentDesiredSpeed >= creatureMovement.currentSpeed);

                if (isAccelerating)
                {
                    creatureMovement.currentSpeed += creatureInfo.runningSpeed / creatureInfo.accelerationTime * deltaTime;
                    creatureMovement.currentSpeed = math.min(creatureMovement.currentSpeed, currentDesiredSpeed);
                }
                else
                {
                    creatureMovement.currentSpeed -= creatureInfo.runningSpeed / creatureInfo.stoppingTime * deltaTime;
                    creatureMovement.currentSpeed = math.max(creatureMovement.currentSpeed, currentDesiredSpeed);
                }


                float rotationAcceleration = math.PI / creatureInfo.rotationTime * deltaTime;
                var rotationVector = (creatureMovement.desiredDirection - creatureMovement.currentDirection) * rotationAcceleration;

                creatureMovement.currentDirection = math.normalize(creatureMovement.currentDirection + rotationVector);
                
                float angle = -math.atan2(creatureMovement.currentDirection.x, creatureMovement.currentDirection.y);
                localTransform.Rotation.value = quaternion.RotateZ(angle).value;
                
                var movementVector = creatureMovement.currentDirection * creatureMovement.currentSpeed;
                float2 newPosition = localTransform.Position.xy + movementVector * deltaTime;
                localTransform.Position.xy = newPosition;

            }

            private float getDesiredSpeed(ref CreatureMovementType desiredMovementType, in CreatureInfoComponent creatureInfo)
            {
                switch (desiredMovementType)
                {
                    case CreatureMovementType.STAY:
                        return 0;
                    case CreatureMovementType.MOVE:
                        return creatureInfo.movementSpeed;
                }

                return 0;
            }
        }
    }
}