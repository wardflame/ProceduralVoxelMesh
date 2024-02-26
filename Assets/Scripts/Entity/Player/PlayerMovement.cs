using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Synthetic.Entity.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public PlayerKernel kernel;
        public CharacterController controller;

        private Vector2 input;

        private void Awake()
        {
            Debug.Log("Movement");
            kernel = GetComponent<PlayerKernel>();

            kernel.input.Movement.Move.performed += OnMove;
            kernel.input.Movement.Move.canceled += OnMove;
        }

        private void FixedUpdate()
        {
            
        }

        private void OnMove(InputAction.CallbackContext value)
        {
            input = value.ReadValue<Vector2>();
        }
    }
}
