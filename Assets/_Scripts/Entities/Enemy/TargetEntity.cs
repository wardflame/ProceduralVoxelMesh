using Essence.Entities.Generic;
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

        public GameObject arrow;
        public float floatSpeed;

        private bool disabled;

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
            if (disabled)
            {
                timer += Time.deltaTime;
                if (timer >= cooldown)
                {
                    timer = 0;
                    meshRenderer.material = normalMaterial;
                    arrow.SetActive(true);
                    disabled = false;
                }
            }
            else
            {
                float y = Mathf.PingPong(Time.time * floatSpeed, 1) * 2 - 1;
                arrow.transform.position = new Vector3(transform.position.x, transform.position.y + 2f + y, transform.position.z);
            }
        }

        private void OnHit()
        {
            if (disabled) return;

            disabled = true;
            meshRenderer.material = shotMaterial;
            arrow.SetActive(false);
            TargetHit?.Invoke();
        }
    }
}
