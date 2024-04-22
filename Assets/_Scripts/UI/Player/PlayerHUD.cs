using Essence.Entities.Player;
using UnityEngine;

namespace Essence.UI.Player
{
    public class PlayerHUD : MonoBehaviour
    {
        public PlayerKernel player;
        public Canvas canvas;
        public RectTransform rectTransform;
        public PlayerPrompt prompt;

        private void Awake()
        {
            Debug.Log("HUD");

            canvas = GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
            prompt = GetComponentInChildren<PlayerPrompt>();
        }
    }
}
