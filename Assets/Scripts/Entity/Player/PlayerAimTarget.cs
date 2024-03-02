using Essence.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence
{
    public class PlayerAimTarget : MonoBehaviour
    {
        public PlayerKernel kernel;
        private Transform mainCamTrans;

        public Transform aimTarget;

        [SerializeField] private float rayDistance = 200;
        [SerializeField] private LayerMask hitLayers;

        [SerializeField] private float smoothTime = 0.1f;

        private Vector3 velocity;

        private void Awake()
        {
            kernel = GetComponent<PlayerKernel>();
            mainCamTrans = kernel.cameraMain.transform;
        }

        private void Update()
        {
            SeekAimTargetCollision();
        }

        private void SeekAimTargetCollision()
        {
            if (Physics.Raycast(kernel.cameraMain.transform.position, kernel.cameraMain.transform.forward, out RaycastHit hit, rayDistance, hitLayers))
            {
                var currentPos = aimTarget.position;
                aimTarget.position = Vector3.SmoothDamp(currentPos, hit.point, ref velocity, smoothTime);
            }
            else
            {
                var currentPos = aimTarget.position;
                aimTarget.position = Vector3.SmoothDamp(currentPos, mainCamTrans.forward * rayDistance, ref velocity, smoothTime);
            }
        }
    }
}
