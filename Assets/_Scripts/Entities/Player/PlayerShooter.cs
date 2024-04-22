using Essence.Entities.Generic;
using Essence.Weapons;
using UnityEngine.InputSystem;

namespace Essence.Entities.Player
{
    public class PlayerShooter : EntityShooter
    {
        public PlayerKernel playerKernel;

        private void Awake()
        {
            playerKernel = GetComponent<PlayerKernel>();
            playerKernel.input.Combat.PrimarySlot.performed += OnPrimarySlotPerformed;
            playerKernel.input.Combat.SecondarySlot.performed += OnSecondarySlotPerformed;
            playerKernel.input.Combat.TertiarySlot.performed += OnTertiarySlotPerformed;
            playerKernel.input.Combat.Fire.performed += OnFirePerformed;
            playerKernel.input.Combat.Fire.canceled += OnFireCanceled;
        }

        private void OnFirePerformed(InputAction.CallbackContext context)
        {
            if (currentWeapon) canFire = true;
        }

        private void OnFireCanceled(InputAction.CallbackContext context)
        {
            canFire = false;
        }

        private void OnPrimarySlotPerformed(InputAction.CallbackContext context)
        {
            SwitchWeapon(WeaponType.Primary);
        }

        private void OnSecondarySlotPerformed(InputAction.CallbackContext context)
        {
            SwitchWeapon(WeaponType.Secondary);
        }

        private void OnTertiarySlotPerformed(InputAction.CallbackContext context)
        {
            SwitchWeapon(WeaponType.Tertiary);
        }
    }
}
