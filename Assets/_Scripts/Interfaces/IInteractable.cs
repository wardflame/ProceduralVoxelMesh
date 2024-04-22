using UnityEngine;

namespace Essence.Interfaces
{
    public interface IInteractable
    {
        public string Prompt { get; }

        public bool CanInteract { get; }

        public void OnInteract(GameObject interactor);
    }
}
