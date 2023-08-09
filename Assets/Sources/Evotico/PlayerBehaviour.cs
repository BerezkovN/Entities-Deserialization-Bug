using System.Collections;
using System.Collections.Generic;
using Sources.Evotico;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Enlighten.Evotico
{
    public class PlayerBehaviour : MonoBehaviour
    {
        public Image Joystick;
        
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
            
            //HandleJoystick();
        }

        private void HandleJoystick()
        {
            var movementSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerMovementSystem>();

            if (!movementSystem.IsMoving)
            {
                Joystick.enabled = false;
                return;
            }

            if (Joystick.enabled)
            {
                return;
            }
            
            var position = movementSystem.StartPosition;
            
            Joystick.enabled = true;
            Joystick.transform.position = new Vector3(position.x, position.y);
            
            Debug.Log("Joystick.transform.position");
        }
    }

}
