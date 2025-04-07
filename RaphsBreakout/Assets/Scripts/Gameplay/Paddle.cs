using System;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilities;

namespace Gameplay
{
    public class Paddle : MonoBehaviour
    {
        private float MaxSpeed => _data?.MaxSpeed ?? 10f;
        private float MoveForce => _data?.MoveForce ?? 5f;
        [GetComponent] private Rigidbody2D _rb;

        private float _currentSpeed;
        private bool _movePerformedThisFrame;
        private PaddleSettingsData _data;

        public void Setup(PaddleSettingsData data)
        {
            _data = data.Clone();
            transform.localScale = _data.GetInitialSize();
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
    }
}
