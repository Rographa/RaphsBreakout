using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Settings/Ball Settings", fileName = "BallSettings")]
    public class BallSettingsData : DataObject
    {
        [SerializeField] private float initialSize;
        [SerializeField] private float initialSpeed;
        [SerializeField] private float bumpSpeedMultiplier;
        [SerializeField] private float maxSpeed;
        [SerializeField] private int damage;
        public float InitialSize => initialSize;
        public float InitialSpeed => initialSpeed;
        public float BumpSpeedMultiplier => bumpSpeedMultiplier;
        public float MaxSpeed => maxSpeed;
        public int Damage => damage;
    }
}