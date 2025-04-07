using System;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private float initialSpeed;
        [GetComponent] private Rigidbody2D _rb;

        private Vector2 _currentSpeed;
        private void Start()
        {
            _rb.linearVelocity = new Vector2(Random.value, Random.value) * initialSpeed;
        }

        private void FixedUpdate()
        {
            _currentSpeed = _rb.linearVelocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Reflect(other.GetContact(0).normal);
        }

        private void Reflect(Vector2 normal)
        {
            //var velocity = _currentSpeed + normal * 2.0f * _currentSpeed.magnitude;
            var velocity = Vector2.Reflect(_currentSpeed, normal);
            _rb.linearVelocity = velocity;
        }
    }
}
