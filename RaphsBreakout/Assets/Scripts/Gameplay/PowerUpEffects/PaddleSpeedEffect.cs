using Enums;
using UnityEngine;

namespace Gameplay.PowerUpEffects
{
    [CreateAssetMenu(menuName = "Game Design/Power-ups/Effects/Paddle Speed Effect")]
    public class PaddleSpeedEffect : PowerUpEffect
    {
        [SerializeField] private float speedMultiplier = 1.1f;
        [field: SerializeField] public override EffectTarget Target { get; set; }
        public override void Apply(GameObject target)
        {
            var paddle = target.GetComponent<Paddle>();
            paddle.AddSpeedMultiplier(speedMultiplier);
        }

        public override void Remove(GameObject target)
        {
            var paddle = target.GetComponent<Paddle>();
            paddle.RemoveSpeedMultiplier(speedMultiplier);
        }
    }
}