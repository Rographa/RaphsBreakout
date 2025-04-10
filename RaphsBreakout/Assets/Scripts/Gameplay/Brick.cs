using Interfaces;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Gameplay
{
    public class Brick : MonoBehaviour, IColorable, IDamageable
    {
        [GetComponent] private SpriteRenderer _spriteRenderer;
        [field : SerializeField] public ColorableId Id { get; set; }
        public UnityEvent onHealthChanged;
        private BrickData _data;
        public int Health 
        {
            get => _currentHealth;
            set
            {
                if (value == _currentHealth) return;
                _currentHealth = value;
                OnHealthChanged();
            }
        }
        private int _currentHealth;

        public Brick Setup(BrickData data)
        {
            _data = data.Clone();
            SetMaxHealth();
            return this;
        }

        private void SetMaxHealth()
        {
            _currentHealth = _data.Health;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0) Die();
        }

        private void OnHealthChanged()
        {
            UpdateColorableId();
            onHealthChanged?.Invoke();
        }

        private void UpdateColorableId()
        {
            var colorableId = _data.GetColorableId(_currentHealth);
            if (Id != colorableId)
            {
                Id = colorableId;
                GameManager.Instance.RequestColor(this);
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}