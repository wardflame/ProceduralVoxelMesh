using Essence.Entities.Player;
using Essence.Interfaces;
using UnityEngine;

namespace Essence.Weapons
{
    public class Weapon : MonoBehaviour, IInteractable
    {
        public WeaponType type;
        public Transform firePoint;
        public WeaponMagazine magazine;
        public WeaponReceiver receiver;

        public ParticleSystem fireFX;

        public void FireWeapon()
        {
            Instantiate(magazine.currentAmmo, firePoint.position, firePoint.rotation);
            fireFX.Play();
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
