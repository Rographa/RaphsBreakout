using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class PowerUpText : MonoBehaviour
    {
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float activeDuration;
        [SerializeField] private float fadeOutDuration;

        [SerializeField] private Ease fadeInEase;
        [SerializeField] private Ease fadeOutEase;
        
        [GetComponent] private TextMeshPro _text;
        [GetComponent] private JumpingTextAnimation _jumpingTextAnimation;

        public void Setup(string text)
        {
            _text.SetText(text);
            _jumpingTextAnimation.enabled = true;
            FadeIn();
        }

        private void FadeIn()
        {
            transform.DOScale(1, fadeInDuration).SetEase(fadeInEase).OnComplete(FadeInFinished);
        }

        private void FadeInFinished()
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSecondsRealtime(activeDuration);
                FadeOut();
            }
        }

        private void FadeOut()
        {
            transform.DOScale(0, fadeInDuration).SetEase(fadeOutEase).OnComplete(FadeOutFinished);
        }

        private void FadeOutFinished()
        {
            Destroy(gameObject);
        }
    }
}
