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
        
        private CreatureMovementType currentMovementState = CreatureMovementType.STAY;
        private CreatureMovementType movementState = CreatureMovementType.MOVE;
        
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
#if UNITY_ANDROID || UNITY_IPHONE
                if (currentPosition.x < Screen.width / 2f)
                {
                    return;
                }
#endif
                
                startPosition = currentPosition;
                isMoving = true;
                currentMovementState = movementState;
            };

            playerInput.Player.EndClick.performed += ctx =>
            {
                startPosition = float2.zero;
                isMoving = false;
                currentMovementState = CreatureMovementType.STAY;
            };

            playerInput.Player.CurrentPosition.performed += ctx => 
            {
                currentPosition = ctx.ReadValue<Vector2>();

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
                    movement.desiredMovementType = this.currentMovementState;
                    movement.desiredDirection = this.direction;
                    movement.isMoving = this.isMoving;
                }).
                WithoutBurst().
                Run();
        }
        

    }

}