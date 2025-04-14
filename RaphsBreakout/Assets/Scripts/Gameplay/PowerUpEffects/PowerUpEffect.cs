using System;
using Enums;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay.PowerUpEffects
{
    [Serializable]
    public abstract class PowerUpEffect : DataObject
    {
        public abstract EffectTarget Target { get; set; }
        public abstract void Apply(GameObject target);
        public abstract void Remove(GameObject target);

    }
    
}