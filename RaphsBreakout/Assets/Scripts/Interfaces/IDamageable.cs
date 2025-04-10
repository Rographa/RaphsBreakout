using UnityEngine.Events;

namespace Interfaces
{
    public interface IDamageable
    {
        public int Health { get; set; }
        public void TakeDamage(int amount);
        public void Die();
    }
}