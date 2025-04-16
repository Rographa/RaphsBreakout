using System.Collections.Generic;
using Gameplay;
using Gameplay.PowerUpEffects;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Power-ups/Power-up Data", fileName = "PowerUp")]
    public class PowerUpData : DataObject
    {
        [SerializeField] private string powerUpName;

        [SerializeReference] private List<PowerUpEffect> effects = new();

        public string PowerUpName => powerUpName;
        public List<PowerUpEffect> Effects => effects;
    }
}