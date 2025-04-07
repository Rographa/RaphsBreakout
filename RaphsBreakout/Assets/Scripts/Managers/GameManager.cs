using Gameplay;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameSettingsData gameSettings;
        [SerializeField] private Paddle paddle;
        protected override void Init()
        {
            base.Init();
            SetupPaddle();
        }

        private void SetupPaddle()
        {
            paddle.Setup(gameSettings.paddleSettings);
        }
    }
}
