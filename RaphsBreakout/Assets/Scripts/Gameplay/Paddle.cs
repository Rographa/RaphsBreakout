using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilities;

namespace Gameplay
{
    public class Paddle : MonoBehaviour, IColorable
    {
        private float MaxSpeed => _data?.MaxSpeed ?? 10f;
        private float MoveForce => _data?.MoveForce ?? 5f;
        [GetComponent] private Rigidbody2D _rb;
        [GetComponent] private SpriteRenderer _renderer;
        [GetComponent] private BoxCollider2D _boxCollider;

        private float _currentSpeed;
        private bool _movePerformedThisFrame;
        private PaddleSettingsData _data;
        [field: SerializeField] public ColorableId Id { get; set; }

        private List<float> _sizeMultiplierList = new();
        private Vector3 _initialSize;

        public void Setup(PaddleSettingsData data)
        {
            _data = data.Clone();
            _initialSize = _data.GetInitialSize();
            SetSize(_initialSize);
        }

        public void SetColor(Color color)
        {
            _renderer.color = color;
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
            var currentSize = _sizeMultiplierList.Aggregate(_initialSize, (current, sizeMultiplier) => current * sizeMultiplier);
            SetSize(currentSize);
        }

        private void SetSize(Vector3 size)
        {
            _renderer.transform.localScale = size;
            _boxCollider.size = size;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movePerformedThisFrame = false;
            var direction = context.ReadValue<Vector2>();
            switch (context.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    _currentSpeed = 0f;
                    break;
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                    _currentSpeed = direction.x * MoveForce;
                    _movePerformedThisFrame = true;
                    break;
                case InputActionPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }

        private void FixedUpdate()
        {
            if (_currentSpeed != 0f)
                Move(_currentSpeed);
        }

        private void LateUpdate()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (_movePerformedThisFrame) return;
            _currentSpeed = 0f;
        }

        private void Move(float speed)
        {
            var finalSpeed = Mathf.Clamp(_rb.linearVelocityX + speed * Time.fixedDeltaTime, -MaxSpeed, MaxSpeed);
            _rb.linearVelocityX = finalSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var collectable = other.GetComponent<ICollectable>();
            collectable ??= other.GetComponentInParent<ICollectable>();
            if (collectable != null)
            {
                collectable.Collect();
            }
        }
    }
}
