using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.PowerUpEffects
{
    [CreateAssetMenu(menuName = "Game Design/Power-ups/Effects/Rage Ball Effect")]
    public class RageBallEffect : PowerUpEffect
    {
        [SerializeField] private Color rageColor;
        [SerializeField] private int bonusDamage;
        [field: SerializeField] public override EffectTarget Target { get; set; }
        public override void Apply(GameObject target)
        {
            var ball = target.GetComponent<Ball>();
            if (ball == null) return;
            ball.SetColor(rageColor);
            ball.AddBonusDamage(bonusDamage);
        }

        public override void Remove(GameObject target)
        {
            var ball = target.GetComponent<Ball>();
            if (ball == null) return;
            ball.ResetColor();
            ball.RemoveBonusDamage(bonusDamage);
        }
    }
}