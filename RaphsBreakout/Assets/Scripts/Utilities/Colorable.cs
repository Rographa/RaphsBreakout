using System;
using Interfaces;
using Managers;
using UnityEngine;

namespace Utilities
{
    public class Colorable : MonoBehaviour, IColorable
    {
        [SerializeField] private Renderer _renderer;
        [field: SerializeField] public ColorableId Id { get; set; }

        private bool _hasSetColor;
        private void OnEnable()
        {
            CheckColor();
        }

        private void CheckColor()
        {
            if (_hasSetColor) return;
            SetColor(GameManager.Instance.GetColor(Id));
        }

        public void SetColor(Color color)
        {
            switch (_renderer)
            {
                case SpriteRenderer spriteRenderer:
                    _hasSetColor = true;
                    spriteRenderer.color = color;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}