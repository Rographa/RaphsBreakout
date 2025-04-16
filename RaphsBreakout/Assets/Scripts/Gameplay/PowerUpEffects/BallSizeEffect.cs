using Enums;
using UnityEngine;

namespace Gameplay.PowerUpEffects
{
    [CreateAssetMenu(menuName = "Game Design/Power-ups/Effects/Ball Size Effect")]
    public class BallSizeEffect : PowerUpEffect
    {
        [SerializeField] private float sizeMultiplier = 1.2f;
        [field: SerializeField] public override EffectTarget Target { get; set; }
        public override void Apply(GameObject target)
        {
            var ball = target.GetComponent<Ball>();
            ball.AddSizeMultiplier(sizeMultiplier);
        }

        public override void Remove(GameObject target)
        {
            var ball = target.GetComponent<Ball>();
            ball.RemoveSizeMultiplier(sizeMultiplier);
        }
    }
}