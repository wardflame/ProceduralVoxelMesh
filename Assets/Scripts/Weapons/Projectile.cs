using Essence.Entities;
using Essence.Entities.Player;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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

        private Vector3 startPosition;
        private Vector3 startForward;

        private Vector3 previousPosition;
        private Vector3 currentPosition;

        private float timer;

        private bool hasHit;

        private int environmentLayer;

        private void Awake()
        {
            startPosition = transform.position;
            startForward = transform.forward;

            currentPosition = startPosition;
            previousPosition = startPosition;
        }

        private void FixedUpdate()
        {
            if (!hasHit) MoveProjectile();
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
                }
            }

            if (hasHit) return;

            if (RaycastBetweenVectors(previousPosition, currentPosition, out RaycastHit currentHit))
            {
                Debug.Log("HIT PREVIOUS!");
                OnHit(currentHit);
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
            hasHit = true;

            transform.position = hit.point;

            // Environment check
            if (hit.collider.gameObject.layer == LayerManager.INT_VOXEL)
            {
                var voxels = Physics.OverlapSphere(transform.position, impactRadius, LayerMask.GetMask("Voxel"));

                if (voxels.Length > 0)
                {
                    foreach (var voxel in voxels)
                    {
                        voxel.attachedRigidbody.isKinematic = false;
                        var force = Vector3.Reflect((currentPosition - hit.point).normalized, hit.normal) * impaceForce;
                        Debug.DrawRay(hit.point, force, Color.magenta, 2);
                        voxel.attachedRigidbody.AddForce(force, ForceMode.Impulse);
                        Destroy(voxel.gameObject, 1);
                    }
                }
            }

            // Entity check
            Entity entity = hit.collider.gameObject.GetComponentInParent<Entity>();
            if (entity) entity.DamageHealth(damage);

            // FX COROUTINE

            StartCoroutine(DestroyProjectile());
        }

        private IEnumerator DestroyProjectile()
        {
            yield return new WaitForSeconds(.025f);

            Destroy(gameObject);
        }
    }
}
