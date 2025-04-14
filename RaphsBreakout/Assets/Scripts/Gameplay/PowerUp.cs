using System;
using DG.Tweening;
using Interfaces;
using Managers;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay
{
    public class PowerUp : MonoBehaviour, IColorable, ICollectable
    {
        [SerializeField] private float fallSpeed = 2f;
        [SerializeField] private SpriteRenderer background;
        private PowerUpData _data;
        public ColorableId Id { get; set; }

        public PowerUp Setup(PowerUpData data)
        {
            _data = data;
            return this;
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector2.down * (fallSpeed * Time.fixedDeltaTime));
        }

        public void SetColor(Color color)
        {
            background.color = color;
        }

        public void Collect()
        {
            LevelManager.ApplyPowerUp(_data);
            Destroy(this.gameObject);
        }
    }
}
