using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Enlighten.Evotico
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private EntityQuery playerQuery;

        public void Awake()
        {
            EntityManager entityManger = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerQuery = entityManger.CreateEntityQuery(new ComponentType[] { ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadOnly<LocalTransform>() });
        }

        public void Update()
        {
            var transforms = playerQuery.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.Temp);
            this.transform.position = transforms[0].Position;
            transforms.Dispose();
        }
    }

}
