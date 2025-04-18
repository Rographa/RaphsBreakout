using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    public class ColorTextAnimation : MonoBehaviour
    {
        [SerializeField] private bool unscaled;
        [SerializeField] private float transitionDuration = 2f;
        [SerializeField] private bool randomColors;
        [SerializeField] private List<Color> colors;
        [GetComponent] private TMP_Text _textComponent;

        private Color _currentColor;
        private Color _colorTarget;

        private Coroutine _coroutine;
        private void Start()
        {
            ComponentInjector.InjectComponents(this);
            _currentColor = GetNextColor();
            _colorTarget = GetNextColor();
            AnimateColors();
        }

        private void OnDestroy()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }

        private void AnimateColors()
        {
            _coroutine = StartCoroutine(AnimateColorsRoutine());
        }

        private IEnumerator AnimateColorsRoutine()
        {
            while (gameObject.activeInHierarchy)
            {
                for (var i = 0f; i < 1f; i += (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) / transitionDuration)
                {
                    _textComponent.color = Color.Lerp(_currentColor, _colorTarget, i);
                    yield return new WaitForEndOfFrame();
                }

                _textComponent.color = _colorTarget;
                _currentColor = _colorTarget;
                _colorTarget = GetNextColor();
            }
        }

        private Color GetNextColor()
        {
            return randomColors
                ? new Color(Random.value, Random.value, Random.value)
                : colors[Random.Range(0, colors.Count)];
        }
    }
}