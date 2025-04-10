using System;
using Interfaces;
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

        private BallSettingsData _data;
        private Vector2 _currentSpeed;
        private float _initialSpeed;

        public void SetupBall(BallSettingsData data)
        {
            ComponentInjector.InjectComponents(this);
            _data = data;
            _initialSpeed = _data.InitialSpeed;
            _rb.linearVelocity = new Vector2(Random.value, Random.value) * _initialSpeed;
        }

        private void FixedUpdate()
        {
            _currentSpeed = _rb.linearVelocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Reflect(other.GetContact(0).normal);

            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1);
            }
        }

        private void Reflect(Vector2 normal)
        {
            //var velocity = _currentSpeed + normal * 2.0f * _currentSpeed.magnitude;
            var velocity = Vector2.Reflect(_currentSpeed, normal) * _data.BumpSpeedMultiplier;
            velocity = velocity.ClampMagnitude(_data.MaxSpeed);
            _rb.linearVelocity = velocity;
        }

        [field: SerializeField] public ColorableId Id { get; set; }
        public void SetColor(Color color)
        {
            
        }
    }
}
