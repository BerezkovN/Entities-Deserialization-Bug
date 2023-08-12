using System;
using System.Runtime.CompilerServices;
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
                float currentDesiredSpeed = getDesiredSpeed(creatureMovement.desiredMovementType, in creatureInfo);

                bool isAccelerating = (currentDesiredSpeed >= creatureMovement.currentSpeed);
                float directionAngle = this.angle(creatureMovement.currentDirection, creatureMovement.desiredDirection);

                if (math.abs(directionAngle) > creatureInfo.skiddingAngle)
                {
                    isAccelerating = false;
                    currentDesiredSpeed = getDesiredSpeed(CreatureMovementType.STAY, in creatureInfo);
                }
                
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
                
                if (math.abs(directionAngle) > 0.1f)
                {
                    float rotationAcceleration = math.PI / creatureInfo.rotationTime * deltaTime;
                    var perpendicular = new float2(creatureMovement.currentDirection.y, -creatureMovement.currentDirection.x);
                    perpendicular *= -math.sign(directionAngle);
                
                    var rotationVector = (perpendicular) * rotationAcceleration;
                    creatureMovement.currentDirection = math.normalize(creatureMovement.currentDirection + rotationVector);
                
                    float finalAngle = -math.atan2(creatureMovement.currentDirection.x, creatureMovement.currentDirection.y);
                    localTransform.Rotation.value = quaternion.RotateZ(finalAngle).value;
                }

                var movementVector = creatureMovement.currentDirection * creatureMovement.currentSpeed;
                float2 newPosition = localTransform.Position.xy + movementVector * deltaTime;
                localTransform.Position.xy = newPosition;

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private float angle(float2 v, float2 w)
            {
                return math.atan2(w.y * v.x - w.x * v.y, w.x * v.x + w.y * v.y);
            }

            private float getDesiredSpeed(CreatureMovementType desiredMovementType, in CreatureInfoComponent creatureInfo)
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