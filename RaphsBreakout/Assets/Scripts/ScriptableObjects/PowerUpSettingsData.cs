using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Settings/Power-up Settings", fileName = "PowerUpSettings")]
    public class PowerUpSettingsData : DataObject
    {
        public List<PowerUpData> powerUps;

        public PowerUpData GetRandom() => powerUps[Random.Range(0, powerUps.Count)];
    }
}