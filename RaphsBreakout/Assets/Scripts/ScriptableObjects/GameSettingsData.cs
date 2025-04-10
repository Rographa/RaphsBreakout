using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings")]
    public class GameSettingsData : ScriptableObject
    {
        public BallSettingsData ballSettings;
        public PaddleSettingsData paddleSettings;
        public ColorSettingsData colorSettingsData;
        
    }
}
