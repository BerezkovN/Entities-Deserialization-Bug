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
        
        private PlayerInput playerInput;

        private float2 startPosition;
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
            
            // Register the callback for when the Move action starts
            playerInput.Player.Move.started += ctx => 
            {
                //startPosition = 
                
                // Get the value of the Vector2 from the context and normalize it
                direction = ctx.ReadValue<Vector2>().normalized;
                isMoving = true;
            };

            // Register the callback for when the Move action ends
            playerInput.Player.Move.canceled += _ => 
            {
                //direction = Vector2.zero;
                isMoving = false;
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