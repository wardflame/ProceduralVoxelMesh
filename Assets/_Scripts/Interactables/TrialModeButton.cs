using Essence.Interfaces;
using UnityEngine;

namespace Essence
{
    public class TrialModeButton : MonoBehaviour, IInteractable
    {
        public string Prompt => "Press E to start trial";

        public bool CanInteract => !TimeTrialGameManager.instance.trialActive;

        public void OnInteract(GameObject interactor)
        {
            TimeTrialGameManager.instance.StartTrial();
        }
    }
}
