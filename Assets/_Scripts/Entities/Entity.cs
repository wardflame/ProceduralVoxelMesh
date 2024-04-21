using UnityEngine;

namespace Essence.Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        public float health;

        private void Awake()
        {
            health = maxHealth;
        }

        public void DamageHealth(float damage)
        {
            health -= damage;

            if (health < 0) health = 0;
            if (health > maxHealth) health = maxHealth;
        }
    }
}
