using Cinemachine;
using Essence.Entities.Generic;
using Essence.Weapons;
using UnityEngine;

namespace Essence.Entities.Player
{
    public class PlayerKernel : EntityKernel
    {
        public EssenceInput input;

        public Camera cameraMain;
        public CinemachineVirtualCamera cameraCMV;

        public Animator animator;
        public CharacterController controller;

        public PlayerMovement movement;
        public PlayerLook look;
        public PlayerAimTarget aimTarget;

        public EntityShooter shooter;

        public PlayerInteractor interactor;

        private void Awake()
        {
            Debug.Log("Kernel");
            input = new EssenceInput();
            movement = GetComponent<PlayerMovement>();
            look = GetComponent<PlayerLook>();
            aimTarget = GetComponent<PlayerAimTarget>();
            animator = GetComponentInChildren<Animator>();
            controller = GetComponent<CharacterController>();
            interactor = GetComponentInChildren<PlayerInteractor>();
            shooter = GetComponent<EntityShooter>();

            input.Enable();
        }
    }
}
