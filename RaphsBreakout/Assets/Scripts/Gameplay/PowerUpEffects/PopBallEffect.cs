using Enums;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.PowerUpEffects
{
    [CreateAssetMenu(menuName = "Game Design/Power-ups/Effects/Pop Ball Effect")]
    public class PopBallEffect : PowerUpEffect
    {
        [SerializeField] private int _ballsToSpawn;

        [field: SerializeField] public override EffectTarget Target { get; set; }

        public override void Apply(GameObject target)
        {
            var ball = target.GetComponent<Ball>();
            if (ball == null) return;
            for (var i = 0; i < _ballsToSpawn; i++)
            {
                var spawnedBall = LevelManager.SpawnBall(ball.transform.position);
                var randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                randomDirection.Normalize();
                spawnedBall.SetVelocity(randomDirection * ball.Speed);
            }

            LevelManager.DestroyBall(ball);
        }

        public override void Remove(GameObject target)
        {
            
        }
    }
}