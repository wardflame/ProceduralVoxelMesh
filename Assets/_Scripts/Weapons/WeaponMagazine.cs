using UnityEngine;

namespace Essence.Weapons
{
    public class WeaponMagazine : MonoBehaviour
    {
        public GameObject currentAmmo;
        public GameObject[] eligibleAmmo;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }
}
