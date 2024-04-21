using Essence.Entities.Generic;
using UnityEngine;

namespace Essence.Entities
{
    [RequireComponent(typeof(EntityHealth))]
    public class EntityKernel : MonoBehaviour
    {
        public EntityHealth Health { get; private set; }

        private void Awake()
        {
            Health = GetComponent<EntityHealth>();
        }
    }
}
