using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Essence.Entity.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public PlayerKernel kernel;
        public CharacterController controller;

        private Vector3 input;

        public float moveSpeed = 4;
        public float acceleration = 1;
        public float deceleration = 1;

        public Vector3 currentSpeed;
        public float inertia;

        private bool isMoving;


        private void Awake()
        {
            Debug.Log("Movement");
            kernel = GetComponent<PlayerKernel>();

            kernel.input.Movement.Move.performed += OnMove;
            kernel.input.Movement.Move.canceled += OnMove;

            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            MoveCharacter();
        }

        private void MoveCharacter()
        {
            Vector3 forwardDir = input.z * kernel.cameraMain.transform.forward.normalized;
            Vector3 rightDir = input.x * kernel.cameraMain.transform.right.normalized;
            var newDir = forwardDir + rightDir;

            if (isMoving)
            {
                // Inertia: Acceleration
                if (currentSpeed.magnitude < newDir.magnitude)
                {
                    inertia += Time.deltaTime * acceleration;
                    currentSpeed = Vector3.Lerp(currentSpeed, newDir, inertia);
                }
                else
                {
                    inertia = 1;
                    currentSpeed = newDir;
                }

                controller.SimpleMove(currentSpeed * moveSpeed);
            }
            else if (!isMoving)
            {
                // Inertia: Deceleration
                if (currentSpeed.magnitude > 0)
                {
                    inertia -= Time.deltaTime * deceleration;
                    currentSpeed = Vector3.Lerp(Vector3.zero, currentSpeed, inertia);

                    controller.SimpleMove(currentSpeed * moveSpeed);
                }
                else
                {
                    inertia = 0;
                    currentSpeed = Vector3.zero;

                    controller.SimpleMove(currentSpeed * moveSpeed);
                }
            }
        }

        private void OnMove(InputAction.CallbackContext value)
        {
            Vector2 axis = value.ReadValue<Vector2>();

            input = new(axis.x, 0, axis.y);

            if (input.magnitude > 0) isMoving = true;
            else isMoving = false;
        }
    }
}
