using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Gameplay/Brick Data", fileName = "BrickData")]
    public class BrickData : DataObject
    {
        [SerializeField] private Brick prefab;
        [SerializeField] private int health;
        [SerializeField] private List<BrickColorData> colorData;

        public ColorableId GetColorableId(int currentHealth)
        {
            var orderedList = colorData.OrderByDescending(x => x.health).ToList();
            foreach (var data in orderedList)
            {
                if (currentHealth < data.health) continue;
                return data.id;
            }

            return default;
        }

        public Brick Spawn(Vector2 position)
        {
            return Spawner.Spawn(prefab, position).Setup(this);
        }

        public int Health => health;
    }
    
    [Serializable]
    public struct BrickColorData
    {
        public ColorableId id;
        public int health;
    }
}
