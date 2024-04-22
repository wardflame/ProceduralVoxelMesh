using UnityEngine;

namespace Essence.Entities.Generic
{
    public class EntityHealth : MonoBehaviour
    {
        private float _health;
        public float HP
        {
            get { return _health; }
            private set
            {
                _health = value;
                if (_health < 0) _health = 0;
                if (_health > maxHealth) _health = maxHealth;
            }
        }

        [SerializeField]
        private float maxHealth;

        private void Awake()
        {
            _health = maxHealth;
        }

        public void DamageHealth(float damage)
        {
            HP -= damage;
        }
    }
}
