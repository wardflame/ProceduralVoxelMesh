using Essence.Entities.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public WeaponType type;
        public Transform firePoint;
        public WeaponMagazine magazine;

        public void FireWeapon()
        {
            Instantiate(magazine.currentAmmo, firePoint.position, firePoint.rotation);
        }
    }

    public enum WeaponType
    {
        Primary,
        Secondary,
        Tertiary
    }
}
