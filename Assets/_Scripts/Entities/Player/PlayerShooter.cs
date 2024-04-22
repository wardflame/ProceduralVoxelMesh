using Essence.Entities.Generic;
using Essence.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Entities.Player
{
    public class PlayerShooter : EntityShooter
    {
        public PlayerKernel kernel;

        private void Awake()
        {
            kernel = GetComponent<PlayerKernel>();
            kernel.input.Combat.PrimarySlot.performed += OnPrimarySlotPerformed;
            kernel.input.Combat.SecondarySlot.performed += OnSecondarySlotPerformed;
            kernel.input.Combat.TertiarySlot.performed += OnTertiarySlotPerformed;
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
