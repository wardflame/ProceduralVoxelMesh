using UnityEngine;
using UnityEngine.UI;

namespace Essence.UI.Player
{
    public class PlayerReticleDynamic : MonoBehaviour
    {
        public PlayerHUD hud;

        private Image sprite;
        private RectTransform rectTransform;

        private void Awake()
        {
            Debug.Log("Dynamic Reticle");

            hud = GetComponentInParent<PlayerHUD>();

            sprite = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            if (hud.player.shooter.currentWeapon != null) FindPointForReticle(hud.player.shooter.currentWeapon.firePoint);
            else rectTransform.anchoredPosition = Vector3.zero;
        }

        private void FindPointForReticle(Transform firePoint)
        {
            Ray firepointRay = new Ray
                (
                    firePoint.position,
                    firePoint.forward
                );

            Debug.DrawRay(firePoint.position, firePoint.forward * 5, Color.blue);

            Vector3 screenPoint;

            if (Physics.Raycast(firepointRay, out RaycastHit hit, 999f, hud.player.aimTarget.hitLayers))
            {
                screenPoint = hud.player.cameraMain.WorldToScreenPoint(hit.point);
            }
            else
            {
                screenPoint = hud.player.cameraMain.WorldToScreenPoint(firepointRay.origin + firepointRay.direction * 200f);
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hud.rectTransform, screenPoint, hud.player.cameraMain, out Vector2 rectPoint))
            {
                rectTransform.anchoredPosition = rectPoint;
            }
        }
    }
}
