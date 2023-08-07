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
                            ComponentType.ReadOnly<CreatureMovementComponent>(),
                            ComponentType.ReadOnly<LocalTransform>()
                        }
                }
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            CreatureMovementJob job = new CreatureMovementJob();
            job.ScheduleParallel(creatureQuery, state.Dependency).Complete();
        }


        [BurstCompile]
        private partial struct CreatureMovementJob : IJobEntity
        {

            public void Execute(ref LocalTransform localTransform, in CreatureMovementComponent creatureMovementComponent)
            {
                float angle = -(float)Math.Atan2(creatureMovementComponent.direction.x, creatureMovementComponent.direction.y);
                
                localTransform.Rotation.value = quaternion.RotateZ(angle).value;
            }
        }
    }
}