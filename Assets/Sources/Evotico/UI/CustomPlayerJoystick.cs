using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace Enlighten.Evotico
{
    public class CustomPlayerJoystick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public GameObject Joystick;
        
        private Transform joystickHead;

        private Image joystickBaseImage;
        private Image joystickHeadImage;
        
        private Transform getTransform()
        {
            return Joystick.transform;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            BeginInteraction(eventData.position, eventData.pressEventCamera);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            MoveStick(eventData.position, eventData.pressEventCamera);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            EndInteraction();
        }

        private void Start()
        {
            joystickHead = getTransform().GetChild(0);
            joystickHeadImage = joystickHead.GetComponent<Image>();
            joystickBaseImage = getTransform().GetComponent<Image>();
            
            m_StartPos = ((RectTransform)getTransform()).anchoredPosition;

            if (m_Behaviour != Behaviour.ExactPositionWithDynamicOrigin) return;
            m_PointerDownPos = m_StartPos;
        }

        private void BeginInteraction(Vector2 pointerPosition, Camera uiCamera)
        {
            var canvasRect = getTransform().parent?.GetComponentInParent<RectTransform>();
            if (canvasRect == null)
            {
                Debug.LogError("TreeStateJoystick needs to be attached as a child to a UI Canvas to function properly.");
                return;
            }

            switch (m_Behaviour)
            {
                case Behaviour.RelativePositionWithStaticOrigin:
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out m_PointerDownPos);
                    break;
                case Behaviour.ExactPositionWithStaticOrigin:
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out m_PointerDownPos);
                    MoveStick(pointerPosition, uiCamera);
                    break;
                case Behaviour.ExactPositionWithDynamicOrigin:
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out var pointerDown);
                    m_PointerDownPos = ((RectTransform)getTransform()).anchoredPosition = pointerDown;
                    break;
            }

            joystickBaseImage.enabled = true;
            joystickHeadImage.enabled = true;

        }

        private void MoveStick(Vector2 pointerPosition, Camera uiCamera)
        {
            var canvasRect = getTransform().parent?.GetComponentInParent<RectTransform>();
            if (canvasRect == null)
            {
                Debug.LogError("TreeStateJoystick needs to be attached as a child to a UI Canvas to function properly.");
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out var position);
            var delta = position - m_PointerDownPos;

            switch (m_Behaviour)
            {
                case Behaviour.RelativePositionWithStaticOrigin:
                    delta = Vector2.ClampMagnitude(delta, movementRange);
                    ((RectTransform)getTransform()).anchoredPosition = (Vector2)m_StartPos + delta;
                    break;

                case Behaviour.ExactPositionWithStaticOrigin:
                    delta = position - (Vector2)m_StartPos;
                    delta = Vector2.ClampMagnitude(delta, movementRange);
                    ((RectTransform)getTransform()).anchoredPosition = (Vector2)m_StartPos + delta;
                    break;

                case Behaviour.ExactPositionWithDynamicOrigin:
                    delta = Vector2.ClampMagnitude(delta, movementRange);
                    ((RectTransform)joystickHead).anchoredPosition = delta;
                    break;
            }

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            SendValueToControl(newPos);
        }

        private void EndInteraction()
        {
            ((RectTransform)getTransform()).anchoredPosition = m_PointerDownPos = m_StartPos;
            ((RectTransform)joystickHead).anchoredPosition = Vector2.zero;
            SendValueToControl(Vector2.zero);
            
            joystickBaseImage.enabled = false;
            joystickHeadImage.enabled = false;
        }

        private void OnPointerDown(InputAction.CallbackContext ctx)
        {
            Debug.Assert(EventSystem.current != null);

            var screenPosition = Vector2.zero;
            if (ctx.control?.device is Pointer pointer)
                screenPosition = pointer.position.ReadValue();

            m_PointerEventData.position = screenPosition;
            EventSystem.current.RaycastAll(m_PointerEventData, m_RaycastResults);
            if (m_RaycastResults.Count == 0)
                return;

            var stickSelected = false;
            foreach (var result in m_RaycastResults)
            {
                if (result.gameObject != gameObject) continue;

                stickSelected = true;
                break;
            }

            if (!stickSelected)
                return;

            BeginInteraction(screenPosition, GetCameraFromCanvas());
        }

        private void OnPointerMove(InputAction.CallbackContext ctx)
        {
            // only pointer devices are allowed
            Debug.Assert(ctx.control?.device is Pointer);

            var screenPosition = ((Pointer)ctx.control.device).position.ReadValue();

            MoveStick(screenPosition, GetCameraFromCanvas());
        }

        private void OnPointerUp(InputAction.CallbackContext ctx)
        {
            EndInteraction();
        }

        private Camera GetCameraFromCanvas()
        {
            var canvas = GetComponentInParent<Canvas>();
            var renderMode = canvas?.renderMode;
            if (renderMode == RenderMode.ScreenSpaceOverlay
                || (renderMode == RenderMode.ScreenSpaceCamera && canvas?.worldCamera == null))
                return null;

            return canvas?.worldCamera ?? Camera.main;
        }
        
        /// <summary>
        /// The distance from the onscreen control's center of origin, around which the control can move.
        /// </summary>
        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }


        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        [Min(0)]
        private float m_MovementRange = 50;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        [SerializeField]
        [Tooltip("Choose how the onscreen stick will move relative to it's origin and the press position.\n\n" +
            "RelativePositionWithStaticOrigin: The control's center of origin is fixed. " +
            "The control will begin un-actuated at it's centered position and then move relative to the pointer or finger motion.\n\n" +
            "ExactPositionWithStaticOrigin: The control's center of origin is fixed. The stick will immediately jump to the " +
            "exact position of the click or touch and begin tracking motion from there.\n\n" +
            "ExactPositionWithDynamicOrigin: The control's center of origin is determined by the initial press position. " +
            "The stick will begin un-actuated at this center position and then track the current pointer or finger position.")]
        private Behaviour m_Behaviour;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        [NonSerialized]
        private List<RaycastResult> m_RaycastResults;
        [NonSerialized]
        private PointerEventData m_PointerEventData;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        /// <summary>Defines how the onscreen stick will move relative to it's origin and the press position.</summary>
        public Behaviour behaviour
        {
            get => m_Behaviour;
            set => m_Behaviour = value;
        }

        /// <summary>Defines how the onscreen stick will move relative to it's center of origin and the press position.</summary>
        public enum Behaviour
        {
            /// <summary>The control's center of origin is fixed in the scene.
            /// The control will begin un-actuated at it's centered position and then move relative to the press motion.</summary>
            RelativePositionWithStaticOrigin,

            /// <summary>The control's center of origin is fixed in the scene.
            /// The control may begin from an actuated position to ensure it is always tracking the current press position.</summary>
            ExactPositionWithStaticOrigin,

            /// <summary>The control's center of origin is determined by the initial press position.
            /// The control will begin unactuated at this center position and then track the current press position.</summary>
            ExactPositionWithDynamicOrigin
        }
    }
}
