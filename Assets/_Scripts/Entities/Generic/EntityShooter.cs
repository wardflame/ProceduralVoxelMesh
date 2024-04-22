using Essence.Weapons;
using UnityEngine;

namespace Essence.Entities.Generic
{
    public class EntityShooter : MonoBehaviour
    {
        public EntityKernel kernel;

        public Transform handSlot;

        public GameObject currentWeaponGOB;
        public Weapon currentWeapon;

        [SerializeField]
        private GameObject[] idleWeapons = new GameObject[3];

        public Transform[] idleSlots = new Transform[3];

        protected bool canFire;
        protected float timer;

        private void Start()
        {
            kernel = GetComponent<EntityKernel>();
            if (currentWeaponGOB != null)
            {
                currentWeapon = currentWeaponGOB.GetComponent<Weapon>();
                currentWeapon.tag = kernel.gameObject.tag;
            }
        }

        protected virtual void Update()
        {
            if (canFire)
            {
                if (timer <= 0)
                {
                    currentWeapon.FireWeapon();
                    timer = currentWeapon.receiver.cycleRate;
                    kernel.animator.SetTrigger("Fire");
                }
            }

            timer -= Time.deltaTime;
            if (timer < 0) timer = 0;
        }

        public void FireWeapon()
        {
            Instantiate(currentWeapon.magazine.currentAmmo, currentWeapon.firePoint.position, currentWeapon.firePoint.rotation);
        }

        public void AddWeapon(Weapon weapon)
        {
            if (currentWeapon == null)
            {
                currentWeaponGOB = weapon.gameObject;
                currentWeapon = currentWeaponGOB.GetComponent<Weapon>();
                PlaceWeaponInHand(currentWeaponGOB);
            }
            else
            {
                idleWeapons[(int)weapon.type] = weapon.gameObject;
                PlaceWeaponInSlot(weapon.gameObject, (int)weapon.type);
            }
        }

        public void SwitchWeapon(WeaponType type)
        {
            var idleGOB = idleWeapons[(int)type];
            var prevGOB = currentWeaponGOB;

            currentWeapon = null;
            currentWeaponGOB = null;
            idleWeapons[(int)type] = null;

            currentWeaponGOB = idleGOB;
            if (currentWeaponGOB != null)
            {
                PlaceWeaponInHand(currentWeaponGOB);
                currentWeapon = currentWeaponGOB.GetComponent<Weapon>();
            }

            if (prevGOB != null)
            {
                PlaceWeaponInSlot(prevGOB, (int)prevGOB.GetComponent<Weapon>().type);
                idleWeapons[(int)prevGOB.GetComponent<Weapon>().type] = prevGOB;
            }
        }

        public void PlaceWeaponInHand(GameObject weaponGOB)
        {
            weaponGOB.transform.SetParent(handSlot);
            weaponGOB.transform.position = handSlot.position;
            weaponGOB.transform.rotation = handSlot.rotation;
        }

        public void PlaceWeaponInSlot(GameObject weaponGOB, int index)
        {
            Transform idleSlot = idleSlots[index];

            weaponGOB.transform.SetParent(idleSlot);
            weaponGOB.transform.position = idleSlot.position;
            weaponGOB.transform.rotation = idleSlot.rotation;
        }
    }
}
