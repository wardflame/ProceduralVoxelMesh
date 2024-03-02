using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Entity.Player
{
    public class PlayerLook : MonoBehaviour
    {
        public PlayerKernel kernel;

        public float lookSensitivity = 1;
        private float rotationPowerCoefficient = 100;
        private float rotationPower => lookSensitivity * rotationPowerCoefficient;

        public int angleThresholdBottom = 290;
        public int angleThresholdTop = 70;

        private Vector2 lookInput;

        private float rotX;
        private float rotY;

        public float smoothTime = 0.1f;
        private float velocityY;
        private float velocityX;

        private bool isLooking;

        private void Awake()
        {
            Debug.Log("Look");

            kernel = GetComponent<PlayerKernel>();

            kernel.input.Movement.Look.performed += OnLookPerformed;
            kernel.input.Movement.Look.canceled += OnLookCanceled;

            rotX = transform.localEulerAngles.x;
            rotY = transform.localEulerAngles.y;
        }

        private void Update()
        {
            if (isLooking) RotatePlayer();
        }

        private void RotatePlayer()
        {
            lookInput = kernel.input.Movement.Look.ReadValue<Vector2>();

            var newRotY = rotY + lookInput.x * Time.deltaTime * rotationPower;
            var newRotX = rotX - lookInput.y * Time.deltaTime * (rotationPower / 2);

            rotY = Mathf.SmoothDamp(rotY, newRotY, ref velocityY, smoothTime);
            rotX = Mathf.SmoothDamp(rotX, newRotX, ref velocityX, smoothTime);

            rotX = Mathf.Clamp(rotX, -90f, 90f);

            transform.rotation = Quaternion.Euler(0, rotY, 0);
            kernel.cameraCMV.Follow.rotation = Quaternion.Euler(rotX, rotY, 0);
        }

        private void OnLookPerformed(InputAction.CallbackContext value)
        {
            isLooking = true;
        }

        private void OnLookCanceled(InputAction.CallbackContext value)
        {
            isLooking = false;
        }
    }
}
