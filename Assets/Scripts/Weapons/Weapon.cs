using Essence.Entities.Player;
using System.Collections;
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

            //StartCoroutine(DistanceTick());
        }

        private IEnumerator DistanceTick()
        {
            while (true)
            {
                Debug.Log("Distance = " + Vector3.Distance(firePoint.position, kernel.aimTarget.targetTransform.position));
                yield return new WaitForSeconds(1);
            }
        }

        private void OnFirePerformed(InputAction.CallbackContext value)
        {
            Instantiate(ammoPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
