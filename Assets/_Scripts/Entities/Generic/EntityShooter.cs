using Essence.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence.Entities.Generic
{
    public class EntityShooter : MonoBehaviour
    {
        public Transform weaponHandSlot;

        public GameObject currentWeapon;

        [SerializeField]
        private GameObject[] equippedWeapons = new GameObject[3];

        private void Awake()
        {
            Instantiate(currentWeapon, weaponHandSlot);
        }

        public void FireWeapon()
        {
            
        }

        public void AddWeapon(Weapon weapon)
        {

        }
    }
}
