using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Interfaces;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utilities;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public const string GameScene = "Game";
        public const string MainMenuScene = "MainMenu";

        public static event Action<bool> OnGamePaused;
        public static event Action<string> OnSceneChanged;
        
        public static int BallLayer => LayerMask.NameToLayer("Ball");
        public static int BallInPaddleLayer => LayerMask.NameToLayer("BallInPaddle");
        
        [SerializeField] private GameSettingsData gameSettings;

        private ColorSettingsData ColorSettings => gameSettings.colorSettings;
        public PowerUpSettingsData PowerUpSettings => gameSettings.powerUpSettings;
        public PaddleSettingsData PaddleSettings => gameSettings.paddleSettings;
        public List<LevelData> Levels => gameSettings.levels;

        private bool _isLoadingScene;
        private bool _gamePaused;

        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(transform.parent.gameObject);
                return;
            }
            base.Awake();
        }

        protected override void Init()
        {
            base.Init();
            
            DontDestroyOnLoad(transform.parent.gameObject);
            SetupColors();
        }
        private void SetupColors()
        {
            var targets = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<IColorable>();
            foreach (var target in targets)
            {
                RequestColor(target);
            }
        }

        public Color GetColor(ColorableId id)
        {
            return gameSettings.colorSettings.Get(id);
        }

        public BallSettingsData GetBallData()
        {
            return gameSettings.ballSettings;
        }

        public void RequestColor(IColorable colorable)
        {
            colorable.SetColor(ColorSettings.Get(colorable.Id));
        }
        
        // Input

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TogglePause();
            }
        }

        public void OnFastForward(InputAction.CallbackContext context)
        {
            if (_gamePaused) return;
            switch (context.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    SetTimeScale(2f);
                    break;
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    SetTimeScale(1f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }

        public static void LoadLevel(LevelData level)
        {
            Instance.SetTimeScale(1f);
            LevelManager.SetLevel(level);
            Instance.LoadLevel_Internal();
        }

        public static void RestartLevel()
        {
            Instance.SetTimeScale(1f);
            Instance.LoadLevel_Internal();
        }

        public static void LoadMainMenu()
        {
            Instance.SetTimeScale(1f);
            Instance.LoadMainMenu_Internal();
        }

        private void LoadLevel_Internal()
        {
            if (_isLoadingScene) return;
            StartCoroutine(LoadSceneAsync(GameScene, StartLevel));
        }

        private void StartLevel()
        {
            LevelManager.Instance.StartGame();
        }

        private void LoadMainMenu_Internal()
        {
            if (_isLoadingScene) return;
            StartCoroutine(LoadSceneAsync(MainMenuScene));
        }

        private IEnumerator LoadSceneAsync(string sceneName, Action callback = null)
        {
            var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;
            _isLoadingScene = true;
            
            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
                yield return null;
            }

            if (sceneName == GameScene)
            {
                SetPause(false);
            }

            OnSceneChanged?.Invoke(sceneName);
            _isLoadingScene = false;
            callback?.Invoke();
        }

        public static void TogglePause()
        {
            Instance.TogglePause_Internal();
        }

        private void TogglePause_Internal()
        {
           SetPause(!_gamePaused);
        }

        private void SetPause(bool value)
        {
            _gamePaused = value;
            SetTimeScale(_gamePaused ? 0f : 1f);
            LevelManager.Instance.Paddle.SetInput(!_gamePaused);
            OnGamePaused?.Invoke(_gamePaused);
        }
    }
}
