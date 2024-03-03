using Essence.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public PlayerKernel kernel;

        public Transform firePoint;
        public GameObject ammoPrefab;

        private void Awake()
        {
            kernel = GetComponentInParent<PlayerKernel>();
            kernel.currentWeapon = this;

            kernel.input.Combat.Fire.performed += OnFirePerformed;
        }

        private void OnFirePerformed(InputAction.CallbackContext value)
        {
            var bullet = Instantiate(ammoPrefab, firePoint.position, firePoint.rotation);
            var projectile = bullet.GetComponent<Projectile>();

            if (projectile) projectile.Initialise(firePoint);

            Destroy(bullet, projectile.lifetime);
        }
    }
}
