using System;
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

        private BallSettingsData _data;
        private Vector2 _currentSpeed;
        private float _initialSpeed;
        private int _damage;

        public float Speed => _rb.linearVelocity.magnitude;
        private static readonly string KillzoneTag = "Killzone";

        public void Launch(Vector2 direction)
        {
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
        }

        public void SetupBall(BallSettingsData data, bool spawnedByPaddle = false)
        {
            ComponentInjector.InjectComponents(this);
            _data = data;
            _initialSpeed = _data.InitialSpeed;
            _damage = data.Damage;
            SetSize(_data.InitialSize);
            if (!spawnedByPaddle)
            {
                SetVelocity(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _initialSpeed);
            }
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
                Destroy(this.gameObject);
            }
        }

        private void TryApplyDamage(Collision2D other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            damageable ??= other.gameObject.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
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

        public void SetVelocity(Vector2 velocity)
        {
            _rb.linearVelocity = velocity;
        }
    }
}
