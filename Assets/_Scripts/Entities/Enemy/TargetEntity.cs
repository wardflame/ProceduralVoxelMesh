using Essence.Entities.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence
{
    public class TargetEntity : MonoBehaviour
    {
        public EntityHealth health;

        public float collapseSpeed;
        public float cooldown;
        public float timer;

        public System.Action TargetHit;

        public Material shotMaterial;
        public Material normalMaterial;

        private bool collapsed;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            TimeTrialGameManager.instance.targets.Add(this);
            health.OnHit += OnHit;
        }

        private void Update()
        {
            if (collapsed)
            {
                timer += Time.deltaTime;
                if (timer >= cooldown)
                {
                    timer = 0;
                    meshRenderer.material = normalMaterial;
                    collapsed = false;
                }
            }
        }

        private void OnHit()
        {
            if (collapsed) return;

            collapsed = true;
            meshRenderer.material = shotMaterial;
            TargetHit?.Invoke();
        }
    }
}
