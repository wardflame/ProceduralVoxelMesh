using Essence.Entities.Generic;
using Essence.Entities.Player;
using Essence.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Weapons
{
    public class Weapon : MonoBehaviour, IInteractable
    {
        public WeaponType type;
        public Transform firePoint;
        public WeaponMagazine magazine;

        public void FireWeapon()
        {
            Instantiate(magazine.currentAmmo, firePoint.position, firePoint.rotation);
        }

        public void OnInteract(GameObject interactor)
        {
            var player = interactor.GetComponent<PlayerInteractor>();
            if (player != null)
            {
                player.kernel.shooter.AddWeapon(this);
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public enum WeaponType
    {
        Primary,
        Secondary,
        Tertiary
    }
}
