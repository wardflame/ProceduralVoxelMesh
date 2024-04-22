using Essence.Entities.Generic;
using UnityEngine;

namespace Essence.Entities
{
    [RequireComponent(typeof(EntityHealth))]
    public class EntityKernel : MonoBehaviour
    {
        public Animator animator;

        public EntityHealth Health { get; private set; }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            Health = GetComponent<EntityHealth>();
        }
    }
}
