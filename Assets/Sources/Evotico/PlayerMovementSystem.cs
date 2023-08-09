using Enlighten.Evotico;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Sources.Evotico
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class PlayerMovementSystem : SystemBase
    {
        public float2 StartPosition => startPosition;
        public bool IsMoving => isMoving;
        
        private PlayerInput playerInput;

        private float2 startPosition;
        
        private float2 currentPosition;
        private float2 direction;
        private bool isMoving;

        protected override void OnStartRunning()
        {
            if (playerInput != null)
            {
                return;
            }

            playerInput = new PlayerInput();
            playerInput.Enable();
            
            playerInput.Player.StartClick.performed += ctx =>
            {
                startPosition = currentPosition;
                isMoving = true;
                
                Debug.Log("[PlayerMovementSystem] startPosition: " + startPosition);
            };

            playerInput.Player.EndClick.performed += ctx =>
            {
                startPosition = float2.zero;
                isMoving = false;
            };

            playerInput.Player.CurrentPosition.performed += ctx => 
            {
                currentPosition = ctx.ReadValue<Vector2>();
                Debug.Log("[PlayerMovementSystem] currentPosition: " + currentPosition);

                if (isMoving)
                {
                    direction = currentPosition - startPosition;
                    direction = math.normalizesafe(direction);
                }
            };
        }

        protected override void OnStopRunning()
        {
            playerInput.Disable();
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<PlayerTag, CreatureMovementComponent, LocalToWorld>().ForEach(
                (ref CreatureMovementComponent movement) =>
                {
                    movement.direction = this.direction;
                    movement.isMoving = this.isMoving;
                }).
                WithoutBurst().
                Run();
        }
        

    }

}