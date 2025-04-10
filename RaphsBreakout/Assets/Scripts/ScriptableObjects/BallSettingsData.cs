using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Settings/Ball Settings", fileName = "BallSettings")]
    public class BallSettingsData : DataObject
    {
        [SerializeField] private float initialSpeed;
        [SerializeField] private float bumpSpeedMultiplier;
        [SerializeField] private float maxSpeed;

        public float InitialSpeed => initialSpeed;
        public float BumpSpeedMultiplier => bumpSpeedMultiplier;
        public float MaxSpeed => maxSpeed;
    }
}