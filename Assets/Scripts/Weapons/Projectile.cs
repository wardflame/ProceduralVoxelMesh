using Essence.Entities;
using Essence.Voxel;
using System.Collections;
using UnityEngine;

namespace Essence.Weapons
{
    public class Projectile : MonoBehaviour
    {
        private const float FORCE_GRAVITY = 9.8f;

        public float damage;

        public float impactRadius = 0.25f;
        public float impaceForce = 1f;

        public float velocity;
        public float lifetime;

        public int penetrativePower;

        private Vector3 startPosition;
        private Vector3 startForward;

        private Vector3 previousPosition;
        private Vector3 currentPosition;

        private float timer;

        private bool hasHit;

        private int objectsHit;

        private void Awake()
        {
            startPosition = transform.position;
            startForward = transform.forward;

            currentPosition = startPosition;
            previousPosition = startPosition;
        }

        private void FixedUpdate()
        {
            //if (!hasHit)
                MoveProjectile();
        }

        private void MoveProjectile()
        {
            timer += Time.fixedDeltaTime;

            if (timer > lifetime) Destroy(gameObject);

            currentPosition = FindVectorOnParabola();

            transform.position = currentPosition;

            if (previousPosition != currentPosition)
            {
                if (RaycastBetweenVectors(previousPosition, currentPosition, out RaycastHit previousHit))
                {
                    Debug.Log("HIT CURRENT!");
                    OnHit(previousHit);
                    
                    objectsHit++;

                    if (objectsHit > penetrativePower)
                    {
                        hasHit = true;
                        Destroy(gameObject, .025f);
                        return;
                    }
                }
            }

            if (hasHit) return;

            if (RaycastBetweenVectors(previousPosition, currentPosition, out RaycastHit currentHit))
            {
                Debug.Log("HIT PREVIOUS!");
                OnHit(currentHit);

                objectsHit++;

                if (objectsHit > penetrativePower)
                {
                    hasHit = true;
                    Destroy(gameObject, .025f);
                    return;
                }
            }

            previousPosition = currentPosition;
        }

        private Vector3 FindVectorOnParabola()
        {
            Vector3 point = startPosition + (velocity * timer * startForward);
            Vector3 gravityVector = FORCE_GRAVITY * timer * timer * Vector3.down;
            return point + gravityVector;
        }

        private bool RaycastBetweenVectors(Vector3 startVector, Vector3 endVector, out RaycastHit hit)
        {
            Debug.DrawRay(startVector, endVector - startVector, Color.green, 5);
            return Physics.Raycast(startVector, endVector - startVector, out hit, (endVector - startVector).magnitude);
        }

        private void OnHit(RaycastHit hit)
        {
            transform.position = hit.point;

            hasHit = true;

            switch (hit.collider.tag)
            {
                case "Voxel":
                    var vMan = hit.collider.GetComponent<VoxelMeshManager>();
                    vMan.DisableVoxel(hit.point - hit.normal * .05f);

                    break;

                case "Entity":
                    Entity entity = hit.collider.gameObject.GetComponentInParent<Entity>();
                    if (entity) entity.DamageHealth(damage);

                    break;

                case "Environment":
                    hasHit = true;
                    Destroy(gameObject, .025f);
                    break;
            }
            
            // FX COROUTINE
        }
    }
}
