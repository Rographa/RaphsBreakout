using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        private bool _aimPointMovedThisFrame;
        private PaddleSettingsData _data;
        [field: SerializeField] public ColorableId Id { get; set; }
        [SerializeField] private Transform arrowHolder;
        [SerializeField] private SpriteRenderer[] arrowRenderers;
        
        private List<float> _sizeMultiplierList = new();
        private Vector3 _initialSize;

        private Ball _currentBall;
        private Vector3 _currentAimPosition;
        private bool IsHoldingBall => _currentBall != null;
        private static Vector3 BallSpawnPoint => new Vector3(0f, 0.125f, 0f);

        public void Setup(PaddleSettingsData data)
        {
            _data = data.Clone();
            _initialSize = _data.GetInitialSize();
            SetSize(_initialSize.x);
            SpawnBall();
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

        public void SpawnBall()
        {
            _currentBall = LevelManager.SpawnBallInPaddle(arrowHolder.position);
            _currentBall.Control(arrowHolder.parent);
            FadeInArrow();
        }
        public void RemoveSizeMultiplier(float multiplier)
        {
            _sizeMultiplierList.Remove(multiplier);
            UpdateSize();
        }
        private void UpdateSize()
        {
            var currentSize = _sizeMultiplierList.Aggregate(_initialSize, (current, sizeMultiplier) => current * sizeMultiplier);
            SetSize(currentSize.x);
        }

        private void SetSize(float size)
        {
            var scale = new Vector3(size, _initialSize.y, _initialSize.z);
            _renderer.transform.localScale = scale;
            _boxCollider.size = scale;
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

        public void OnPoint(InputAction.CallbackContext context)
        {
            var aimPoint = context.ReadValue<Vector2>();
            _currentAimPosition = CameraManager.Main.ScreenToWorldPoint(aimPoint);
            switch (context.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    _aimPointMovedThisFrame = true;
                    break;
                case InputActionPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (IsHoldingBall)
            {
                _currentBall.Launch(arrowHolder.up);
                _currentBall = null;
                FadeOutArrow();
            }
        }

        private void FixedUpdate()
        {
            if (_currentSpeed != 0f)
                Move(_currentSpeed);
        }

        private void LateUpdate()
        {
            CheckMoveInput();

            if (IsHoldingBall)
            {
                CheckAimInput();
            }
        }

        private void CheckAimInput()
        {
            if (!_aimPointMovedThisFrame) return;
            var direction = (_currentAimPosition - arrowHolder.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var eulerAngles = arrowHolder.localEulerAngles;
            eulerAngles.z = -90 + angle;
            arrowHolder.localEulerAngles = eulerAngles;
        }

        private void CheckMoveInput()
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

        private void FadeInArrow()
        {
            foreach (var rend in arrowRenderers)
            {
                var newColor = rend.color;
                newColor.a = 1f;
                rend.DOColor(newColor, 0.2f);
            }
        }

        private void FadeOutArrow()
        {
            foreach (var rend in arrowRenderers)
            {
                var newColor = rend.color;
                newColor.a = 0f;
                rend.DOColor(newColor, 0.2f);
            }
        }
    }
}
