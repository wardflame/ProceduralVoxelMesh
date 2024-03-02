using Essence.Entity.Player;
using UnityEngine;

namespace Essence.UI.Player
{
    public class PlayerHUD : MonoBehaviour
    {
        public PlayerKernel player;

        private void Awake()
        {
            Debug.Log("HUD");
        }
    }
}
