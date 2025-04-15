using System.Linq;
using Gameplay;
using Interfaces;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static int BallLayer => LayerMask.NameToLayer("Ball");
        public static int BallInPaddleLayer => LayerMask.NameToLayer("BallInPaddle");
        
        [SerializeField] private GameSettingsData gameSettings;
        [SerializeField] private Paddle paddle;

        private ColorSettingsData ColorSettings => gameSettings.colorSettings;
        public PowerUpSettingsData PowerUpSettings => gameSettings.powerUpSettings;
        protected override void Init()
        {
            base.Init();
            SetupColors();
            SetupPaddle();
        }

        private void SetupPaddle()
        {
            paddle.Setup(gameSettings.paddleSettings);
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
    }
}
