using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class Ball : MonoBehaviour, IColorable
    {
        [GetComponent] private Rigidbody2D _rb;
        [GetComponent] private SpriteRenderer _renderer;
        [GetComponent] private TrailRenderer _trail;
        [GetComponent] private CircleCollider2D _circleCollider;

        private List<float> _sizeMultiplierList = new();
        private BallSettingsData _data;
        private Color _initialColor;
        private Vector2 _currentSpeed;
        private float _initialSpeed;
        private float _initialSize;
        private int _damage;
        private int _bonusDamage;
        private int TotalDamage => _damage + _bonusDamage;
        public float Speed => _rb.linearVelocity.magnitude;
        private static readonly string KillzoneTag = "Killzone";

        public void Launch(Vector2 direction)
        {
            _trail.emitting = true;
            gameObject.layer = GameManager.BallLayer;
            transform.parent = null;
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.WakeUp();
            SetVelocity(direction * _initialSpeed);
        }

        public void Control(Transform parent)
        {
            gameObject.layer = GameManager.BallInPaddleLayer;
            transform.parent = parent;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _trail.emitting = false;
        }

        public void SetupBall(BallSettingsData data, bool spawnedByPaddle = false)
        {
            ComponentInjector.InjectComponents(this);
            _data = data;
            _initialSpeed = _data.InitialSpeed;
            _damage = data.Damage;
            _initialColor = _renderer.color;
            _initialSize = _data.InitialSize;
            SetSize(_initialSize);
            if (!spawnedByPaddle)
            {
                SetVelocity(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _initialSpeed);
            }
        }
        public void AddSizeMultiplier(float multiplier)
        {
            _sizeMultiplierList.Add(multiplier);
            UpdateSize();
        }
        public void RemoveSizeMultiplier(float multiplier)
        {
            _sizeMultiplierList.Remove(multiplier);
            UpdateSize();
        }
        private void UpdateSize()
        {
            var currentSize =
                _sizeMultiplierList.Aggregate(_initialSize, (current, sizeMultiplier) => current * sizeMultiplier);
            SetSize(currentSize);
        }
        public void SetSize(float size)
        {
            _circleCollider.radius = size / 2f;
            _renderer.transform.localScale = Vector3.one * size;
            _trail.startWidth = size;
        }

        private void FixedUpdate()
        {
            if (_rb.linearVelocity.magnitude > 0.1f)
                _currentSpeed = _rb.linearVelocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Reflect(other.GetContact(0).normal);
            TryApplyDamage(other);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(KillzoneTag))
            {
                LevelManager.DestroyBall(this);
            }
        }

        private void TryApplyDamage(Collision2D other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            damageable ??= other.gameObject.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(TotalDamage);
            }
        }

        private void Reflect(Vector2 normal)
        {
            //var velocity = (_currentSpeed + normal * 2.0f * _currentSpeed.magnitude) * _data.BumpSpeedMultiplier;
            var velocity = Vector2.Reflect(_currentSpeed, normal) * _data.BumpSpeedMultiplier;
            velocity = velocity.ClampMagnitude(_data.MaxSpeed);
            SetVelocity(velocity);
        }

        [field: SerializeField] public ColorableId Id { get; set; }
        public void SetColor(Color color)
        {
            _renderer.color = color;
            _trail.startColor = color;
            _trail.endColor = color;
        }

        public void ResetColor()
        {
            SetColor(_initialColor);
        }

        public void SetVelocity(Vector2 velocity)
        {
            _rb.linearVelocity = velocity;
        }

        public void AddBonusDamage(int amount)
        {
            _bonusDamage += amount;
        }

        public void RemoveBonusDamage(int amount)
        {
            _bonusDamage -= amount;
        }
    }
}
