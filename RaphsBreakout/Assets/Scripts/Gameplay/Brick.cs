using DG.Tweening;
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
        [GetComponent] private BoxCollider2D _collider;
        [field : SerializeField] public ColorableId Id { get; set; }
        public UnityEvent onHealthChanged;
        private BrickData _data;
        private Vector3 _originalScale;
        private Tween _wobble;
        private int _initialSortOrder;
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
            UpdateColorableId();
            _originalScale = _spriteRenderer.transform.localScale;
            _initialSortOrder = _spriteRenderer.sortingOrder;
            return this;
        }

        private void SetMaxHealth()
        {
            _currentHealth = _data.Health;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0) DoWobbleAndDisappear();
            else DoWobble();
        }

        private void DoWobbleAndDisappear()
        {
            _collider.enabled = false;
            if (_wobble != null)
            {
                _wobble.Complete(true);
            }
            _spriteRenderer.sortingOrder = _initialSortOrder + 1;
            //_wobble = _spriteRenderer.transform.DOShakeScale(0.2f, _originalScale * 1.1f, 15, 5, true, ShakeRandomnessMode.Harmonic).SetEase(Ease.InBounce).OnComplete(Die);
            _wobble = _spriteRenderer.transform.DOPunchScale(_originalScale * 1.1f, 0.2f).SetEase(Ease.InBounce).OnComplete(Disappear);
        }

        private void Disappear()
        {
            if (_wobble != null)
            {
                _wobble.Complete(true);
            }

            _wobble = _spriteRenderer.transform.DOScale(0, 0.1f).OnComplete(Die);
        }

        private void DoWobble()
        {
            if (_wobble != null)
            {
                _wobble.Complete(true);
            }
            
            //_wobble = _spriteRenderer.transform.DOShakeScale(0.2f, _originalScale * 1.1f).SetEase(Ease.InBounce).OnComplete(ResetScale);
            _spriteRenderer.sortingOrder = _initialSortOrder + 1;
            _wobble = _spriteRenderer.transform.DOPunchScale(_originalScale * 1.1f, 0.2f).SetEase(Ease.InBounce).OnComplete(ResetScale);
        }

        private void ResetScale()
        {
            _spriteRenderer.sortingOrder = _initialSortOrder;
            _spriteRenderer.transform.localScale = _originalScale;
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