using Essence.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Entities.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        public PlayerKernel kernel;

        public IInteractable currentInteractable;

        private void Awake()
        {
            kernel = GetComponentInParent<PlayerKernel>();
            kernel.input.General.Interact.performed += OnInteract;
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (interactable == currentInteractable) currentInteractable = null;
            }
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnInteract(gameObject);
                currentInteractable = null;
            }
        }
    }
}
