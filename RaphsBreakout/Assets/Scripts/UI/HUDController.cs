using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup pausePanel;
        [SerializeField] private CanvasGroup gameOverPanel;
        [SerializeField] private Button[] resumeButtons;
        [SerializeField] private Button[] restartButtons;
        [SerializeField] private Button[] menuButtons;

        private Coroutine _fadeRoutine;

        private void Start()
        {
            canvas.worldCamera = CameraManager.Main;
            SetupButtons();
        }

        private void SetupButtons()
        {
            foreach (var resumeButton in resumeButtons)
            {
                resumeButton.onClick.AddListener(ResumeButton);    
            }

            foreach (var restartButton in restartButtons)
            {
                restartButton.onClick.AddListener(RestartButton);    
            }

            foreach (var menuButton in menuButtons)
            {
                menuButton.onClick.AddListener(MenuButton);
            }
        }

        private void OnEnable()
        {
            HandleSubscription(true);
        }

        private void OnDisable()
        {
            HandleSubscription(false);
        }

        private void HandleSubscription(bool subscribe)
        {
            switch (subscribe)
            {
                case true:
                    GameManager.OnGamePaused += HandlePause;
                    LevelManager.OnGameOver += HandleGameOver;
                    break;
                case false:
                    GameManager.OnGamePaused -= HandlePause;
                    LevelManager.OnGameOver -= HandleGameOver;
                    break;
            }
        }

        private void HandleGameOver()
        {
            gameOverPanel.interactable = true;
            gameOverPanel.blocksRaycasts = true;
            
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(Fade(gameOverPanel, 0f, 1f));
        }

        private void HandlePause(bool paused)
        {
            pausePanel.interactable = paused;
            pausePanel.blocksRaycasts = paused;
            switch (paused)
            {
                case true:
                    FadeInPause();
                    break;
                case false:
                    FadeOutPause();
                    break;
            }
        }
        
        private void ResumeButton()
        {
            GameManager.TogglePause();
        }
        private void RestartButton()
        {
            GameManager.RestartLevel();
        }
        private void MenuButton()
        {
            GameManager.LoadMainMenu();
        }

        private void FadeInPause()
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(Fade(pausePanel, 0f, 1f));
        }

        private void FadeOutPause()
        {
            if (pausePanel.alpha == 0) return;
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(Fade(pausePanel, 1f, 0f, 0.2f));
        }

        private IEnumerator Fade(CanvasGroup target, float startAlpha, float endAlpha, float duration = 1f)
        {
            target.alpha = startAlpha;
            for (var i = 0f; i < 1f; i += Time.unscaledDeltaTime / duration)
            {
                target.alpha = Mathf.Lerp(startAlpha, endAlpha, i);
                yield return new WaitForEndOfFrame();
            }

            target.alpha = endAlpha;
        }
    }
}
