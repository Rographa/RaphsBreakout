using Enums;
using UnityEngine;

namespace Gameplay.PowerUpEffects
{
    [CreateAssetMenu(menuName = "Game Design/Power-ups/Effects/Paddle Size Effect")]
    public class PaddleSizeEffect : PowerUpEffect
    {
        [SerializeField] private float sizeMultiplier = 1.1f;
        [field: SerializeField] public override EffectTarget Target { get; set; }

        public override void Apply(GameObject target)
        {
            var paddle = target.GetComponent<Paddle>();
            paddle.AddSizeMultiplier(sizeMultiplier);
        }

        public override void Remove(GameObject target)
        {
            var paddle = target.GetComponent<Paddle>();
            paddle.RemoveSizeMultiplier(sizeMultiplier);
        }
    }
}