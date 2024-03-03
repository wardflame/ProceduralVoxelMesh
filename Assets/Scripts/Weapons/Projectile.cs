using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Essence.Weapons
{
    public class Projectile : MonoBehaviour
    {
        private const float FORCE_GRAVITY = 9.8f;

        public float velocity;
        public float lifetime;

        private Vector3 startPosition;
        private Vector3 startForward;

        private bool isInitialised;
        private float startTime = -1;

        public void Initialise(Transform startPosition)
        {
            this.startPosition = startPosition.position;
            startForward = startPosition.forward.normalized;
            isInitialised = true;
        }

        private void FixedUpdate()
        {
            if (!isInitialised) return;

            if (startTime < 0) startTime = Time.time;

            RaycastHit hit;
            float currentTime = Time.time - startTime;
            float nextTime = currentTime + Time.fixedDeltaTime;

            Vector3 currentPoint = FindVectorOnParabola(currentTime);
            Vector3 nextPoint = FindVectorOnParabola(nextTime);

            if (RaycastBetweenVectors(currentPoint, nextPoint, out hit))
            {
                
            }
        }

        private void Update()
        {
            if (!isInitialised) return;

            float currentTime = Time.time - startTime;
            Vector3 currentPoint = FindVectorOnParabola(currentTime);
            transform.position = currentPoint;
        }

        private Vector3 FindVectorOnParabola(float time)
        {
            Vector3 point = startPosition + (startForward * velocity * time);
            Vector3 gravityVector = Vector3.down * FORCE_GRAVITY * time * time;
            return point + gravityVector;
        }

        private bool RaycastBetweenVectors(Vector3 startVector, Vector3 endVector, out RaycastHit hit)
        {
            return Physics.Raycast(startVector, endVector - startVector, out hit, (endVector - startVector).magnitude);
        }
    }
}
