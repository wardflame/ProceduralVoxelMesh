using UnityEngine;
using UnityEngine.UI;

namespace Essence.UI.Player
{
    public class PlayerReticleDynamic : MonoBehaviour
    {
        public PlayerHUD hud;

        private Image sprite;
        private RectTransform rectTransform;
        private Transform firePoint => hud.player.currentWeapon.firePoint;

        private void Awake()
        {
            Debug.Log("Dynamic Reticle");

            hud = GetComponentInParent<PlayerHUD>();

            sprite = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
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

            /*screenPoint = hud.player.cameraMain.WorldToScreenPoint(hud.player.aimTarget.targetTransform.position);
            screenPoint.z = 0;*/

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hud.rectTransform, screenPoint, hud.player.cameraMain, out Vector2 rectPoint))
            {
                rectTransform.anchoredPosition = rectPoint;
            }
        }
    }
}
