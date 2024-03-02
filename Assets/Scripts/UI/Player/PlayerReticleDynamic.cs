using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Essence.UI.Player
{
    public class PlayerReticleDynamic : MonoBehaviour
    {
        public PlayerHUD hud;

        private Image sprite;

        private void Awake()
        {
            Debug.Log("Dynamic Reticle");

            hud = GetComponentInParent<PlayerHUD>();
        }

        private void FixedUpdate()
        {
            sprite.rectTransform.localPosition = hud.player.cameraMain.WorldToScreenPoint(hud.player.aimTarget.aimTarget.position, Camera.MonoOrStereoscopicEye.Mono);
        }
    }
}
