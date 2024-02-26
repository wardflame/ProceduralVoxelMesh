using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Entity.Player
{
    public class PlayerLook : MonoBehaviour
    {
        public PlayerKernel kernel;

        public float rotationPower = 1;
        private Vector2 rotation;

        private void Awake()
        {
            Debug.Log("Look");

            kernel = GetComponent<PlayerKernel>();

            kernel.input.Movement.Look.performed += OnLook;
            kernel.input.Movement.Look.canceled += OnLook;
        }

        private void Update()
        {
            Look();
        }

        private void Look()
        {
            transform.rotation *= Quaternion.AngleAxis(rotation.x * rotationPower, Vector3.up);
            kernel.cameraCMV.Follow.transform.rotation *= Quaternion.AngleAxis(-rotation.y * rotationPower, Vector3.right);

            var angles = kernel.cameraCMV.Follow.transform.localEulerAngles;
            angles.z = 0;

            var angle = kernel.cameraCMV.Follow.transform.localEulerAngles.x;

            if (angle > 180 && angle < 340)
            {
                angles.x = 340;
            }
            else if (angle < 180 && angle > 40)
            {
                angles.x = 40;
            }

            kernel.cameraCMV.Follow.transform.localEulerAngles = angles;
        }

        private void OnLook(InputAction.CallbackContext value)
        {
            rotation = value.ReadValue<Vector2>();
        }
    }
}
