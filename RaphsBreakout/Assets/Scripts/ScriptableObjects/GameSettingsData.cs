using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings")]
    public class GameSettingsData : ScriptableObject
    {
        public List<LevelData> levels;
        public BallSettingsData ballSettings;
        public PaddleSettingsData paddleSettings;
        [FormerlySerializedAs("colorSettingsData")] public ColorSettingsData colorSettings;
        public PowerUpSettingsData powerUpSettings;
    }
}
