using Cinemachine;
using UnityEngine;

namespace Essence.Entity.Player
{
    public class PlayerKernel : MonoBehaviour
    {
        public EssenceInput input;

        public Camera cameraMain;
        public CinemachineVirtualCamera cameraCMV;

        public PlayerMovement movement;
        public PlayerLook look;

        private void Awake()
        {
            Debug.Log("Kernel");
            input = new EssenceInput();
            movement = GetComponent<PlayerMovement>();
            look = GetComponent<PlayerLook>();

            input.Enable();
        }
    }
}
