using UnityEngine;

namespace Essence.Entity.Player
{
    public class PlayerAimTarget : MonoBehaviour
    {
        public PlayerKernel kernel;
        private Transform mainCamTrans;

        public Transform targetTransform;

        public LayerMask hitLayers;
        [SerializeField] private float rayDistance = 200;

        [SerializeField] private float smoothTime = 0.1f;

        private Vector3 velocity;

        private void Awake()
        {
            Debug.Log("Aim Target");

            kernel = GetComponent<PlayerKernel>();
            mainCamTrans = kernel.cameraMain.transform;
        }

        private void FixedUpdate()
        {
            SeekAimTargetCollision();
        }

        private void SeekAimTargetCollision()
        {
            if (Physics.Raycast(kernel.cameraMain.transform.position, kernel.cameraMain.transform.forward, out RaycastHit hit, rayDistance, hitLayers))
            {
                var currentPos = targetTransform.position;
                targetTransform.position = Vector3.SmoothDamp(currentPos, hit.point, ref velocity, smoothTime);
            }
            else
            {
                var currentPos = targetTransform.position;
                targetTransform.position = Vector3.SmoothDamp(currentPos, mainCamTrans.forward * rayDistance, ref velocity, smoothTime);
            }
        }
    }
}
