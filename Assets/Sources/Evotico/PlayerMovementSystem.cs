using Enlighten.Evotico;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Sources.Evotico
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [DisableAutoCreation]
    public partial class PlayerMovementSystem : SystemBase
    {
        public float2 StartPosition => startPosition;
        public bool IsMoving => isMoving;
        
        private PlayerInput playerInput;

#if UNITY_ANDROID || UNITY_IPHONE
        private bool isSwiping;
        private float2 swipeStartPosition;
        private float2 currentSwipePosition;
#endif
        
        private float2 startPosition;
        private float2 currentPosition;
        private float2 direction = new float2(0, 1f);
        
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
                if (currentPosition.x < Screen.width / 2f)
                {
                    return;
                }

                startPosition = currentPosition;
                isMoving = true;
                currentMovementState = movementState;
            };

            playerInput.Player.EndClick.performed += ctx =>
            {
                OnSwipeEnd();
                
                startPosition = float2.zero;
                isMoving = false;
                currentMovementState = CreatureMovementType.STAY;
            };

            playerInput.Player.CurrentPosition.performed += ctx => 
            {
                OnSwipeUpdate(ctx);
                currentPosition = ctx.ReadValue<Vector2>();

                if (isMoving)
                {
                    direction = currentPosition - startPosition;
                    direction = math.normalizesafe(direction);
                }
            };
        }

        private void OnSwipeStart()
        {
            isSwiping = true;
            swipeStartPosition = currentPosition;
            Debug.Log("Swipe start " + swipeStartPosition);
        }

        private void OnSwipeUpdate(InputAction.CallbackContext ctx)
        {
            if (!isSwiping) return;
            
            currentSwipePosition = ctx.ReadValue<Vector2>();
        }
        
        private void OnSwipeEnd()
        {
            if (!isSwiping) return;
            
            float swipeDirection = currentSwipePosition.y - swipeStartPosition.y;

            if (swipeDirection > 0)
            {
                movementState =
                    (CreatureMovementType)math.min((int)(movementState + 1), (int)CreatureMovementType.SPRINT);
            }
            else
            {
                movementState =
                    (CreatureMovementType)math.max((int)(movementState - 1), (int)CreatureMovementType.CROUCH);
            }

            currentMovementState = movementState;
            Debug.Log(movementState + " " + currentSwipePosition);

            isSwiping = false;
            swipeStartPosition = float2.zero;
        }

        protected override void OnUpdate()
        {
            HandleSwipe();
            
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

        private void HandleSwipe()
        {
            int touchIndex = isMoving ? 1 : 0;

            if (Touchscreen.current == null)
            {
                return;
            }
            
            TouchControl currentTouch = Touchscreen.current.touches[touchIndex];
            
            if (currentTouch.phase.value == TouchPhase.Began)
            {
                isSwiping = true;
                swipeStartPosition = currentTouch.startPosition.value;
                return;
            }

            if (isSwiping)
            {
                currentSwipePosition = currentTouch.position.value;
            }

            if (currentTouch.phase.value == TouchPhase.Ended)
            {
                OnSwipeEnd();
            }
        }

        protected override void OnStopRunning()
        {
            playerInput.Disable();
        }
    }

}