using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Settings/Paddle Settings", fileName = "PaddleSettings")]
    public class PaddleSettingsData : DataObject
    {
        [SerializeField] private float moveForce;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float initialSize;
        [SerializeField] private int ballsInStock;

        public float MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }

        public float MoveForce
        {
            get => moveForce;
            set => moveForce = value;
        }

        public float InitialSize => initialSize;
        public int BallsInStock => ballsInStock;

        public Vector3 GetInitialSize()
        {
            return new Vector3(initialSize, 0.25f, 1f);
        }
    }
}