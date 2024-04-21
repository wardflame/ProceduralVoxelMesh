using Cinemachine;
using Essence.Weapons;
using UnityEngine;

namespace Essence.Entities.Player
{
    public class PlayerKernel : MonoBehaviour
    {
        public EssenceInput input;

        public Camera cameraMain;
        public CinemachineVirtualCamera cameraCMV;

        public Animator animator;
        public CharacterController controller;

        public PlayerMovement movement;
        public PlayerLook look;
        public PlayerAimTarget aimTarget;

        public Weapon currentWeapon;

        private void Awake()
        {
            Debug.Log("Kernel");
            input = new EssenceInput();
            movement = GetComponent<PlayerMovement>();
            look = GetComponent<PlayerLook>();
            aimTarget = GetComponent<PlayerAimTarget>();
            currentWeapon = GetComponentInChildren<Weapon>();
            animator = GetComponentInChildren<Animator>();
            controller = GetComponent<CharacterController>();

            input.Enable();
        }
    }
}
