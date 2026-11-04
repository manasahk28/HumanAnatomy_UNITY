using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HumanAnatomy
{
    public class OrbitCameraController : MonoBehaviour
    {
        [Header("Target & Distances")]
        public Transform targetTransform;
        public Vector3 targetOffset = new Vector3(0, 0.95f, 0);
        public float distance = 2.5f;
        public float minDistance = 0.4f;
        public float maxDistance = 6.0f;

        [Header("Speeds & Sensitivities")]
        public float xSpeed = 120.0f;
        public float ySpeed = 100.0f;
        public float panSpeed = 0.005f;
        public float zoomSensitivity = 0.003f;
        public float smoothTime = 0.12f;

        [Header("Angle Limits")]
        public float yMinLimit = -60f;
        public float yMaxLimit = 85f;

        private float x = 0.0f;
        private float y = 15.0f;
        private float currentDistance;
        private Vector3 currentTargetPosition;

        private float targetX;
        private float targetY;
        private float targetDistance;
        private Vector3 targetPanOffset;

        private Vector3 initialTargetOffset;
        private float initialDistance;
        private float initialX;
        private float initialY;

        private Vector3 positionVelocity;
        private float distanceVelocity;
        private float xVelocity;
        private float yVelocity;

        private void Start()
        {
            initialTargetOffset = targetOffset;
            initialDistance = distance;

            Vector3 angles = transform.eulerAngles;
            x = targetX = angles.y;
            y = targetY = angles.x;

            initialX = x;
            initialY = y;

            targetDistance = currentDistance = distance;
            targetPanOffset = targetOffset;
            currentTargetPosition = GetPivotPosition();

            transform.position = currentTargetPosition - (transform.rotation * Vector3.forward * currentDistance);
        }

        private void LateUpdate()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            // Orbit (Left Mouse Button)
            if (mouse.leftButton.isPressed && !isOverUI)
            {
                Vector2 delta = mouse.delta.ReadValue();
                targetX += delta.x * xSpeed * 0.02f;
                targetY -= delta.y * ySpeed * 0.02f;
                targetY = ClampAngle(targetY, yMinLimit, yMaxLimit);
            }

            // Pan (Right or Middle Mouse Button)
            if ((mouse.rightButton.isPressed || mouse.middleButton.isPressed) && !isOverUI)
            {
                Vector2 delta = mouse.delta.ReadValue();
                Vector3 right = transform.right;
                Vector3 up = transform.up;
                targetPanOffset -= (right * delta.x + up * delta.y) * panSpeed * (currentDistance * 0.5f);
            }

            // Zoom (Scroll Wheel)
            if (!isOverUI)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    targetDistance -= scroll * zoomSensitivity;
                    targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
                }
            }

            // Smooth Interpolation
            x = Mathf.SmoothDamp(x, targetX, ref xVelocity, smoothTime);
            y = Mathf.SmoothDamp(y, targetY, ref yVelocity, smoothTime);
            currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, smoothTime);
            currentTargetPosition = Vector3.SmoothDamp(currentTargetPosition, GetPivotPosition(), ref positionVelocity, smoothTime);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = currentTargetPosition - (rotation * Vector3.forward * currentDistance);

            transform.rotation = rotation;
            transform.position = position;
        }

        private Vector3 GetPivotPosition()
        {
            Vector3 basePos = targetTransform != null ? targetTransform.position : Vector3.zero;
            return basePos + targetPanOffset;
        }

        public void FocusOn(Vector3 worldPoint, float desiredDistance = 1.0f)
        {
            Vector3 basePos = targetTransform != null ? targetTransform.position : Vector3.zero;
            targetPanOffset = worldPoint - basePos;
            targetDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        }

        public void ResetView()
        {
            targetPanOffset = initialTargetOffset;
            targetDistance = initialDistance;
            targetX = initialX;
            targetY = initialY;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F) angle += 360F;
            if (angle > 360F) angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}

