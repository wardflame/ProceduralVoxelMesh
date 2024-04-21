using Essence.Entities.Player;
using UnityEngine;

namespace Essence.UI.Player
{
    public class PlayerHUD : MonoBehaviour
    {
        public PlayerKernel player;
        public Canvas canvas;
        public RectTransform rectTransform;

        private void Awake()
        {
            Debug.Log("HUD");

            canvas = GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
