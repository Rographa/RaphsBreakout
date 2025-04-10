using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Levels/Level Data", fileName = "LevelData")]
    public class LevelData : DataObject
    {
        [SerializeField] private Vector2Int mapSize = new Vector2Int(16, 10);
        [SerializeField] private Vector2 cellSize = new Vector2(0.5f, 0.25f);
        [SerializeField] private BrickData brickData;
        [Range(0, 40)] public int rowLimit = 8;

        public Vector2 toolRadius;
        //public CellType[,] Grid;
        public List<ListWrapper> Grid;

        public Vector2Int MapSize
        {
            get => mapSize;
            set => mapSize = value;
        }

        public Vector2 CellSize => cellSize;

        public BrickData BrickData => brickData;

        public Vector2Int GetCellMap()
        {
            return new Vector2Int(
                Mathf.CeilToInt(mapSize.x / cellSize.x),
                Mathf.CeilToInt(mapSize.y / cellSize.y)
            );
        }
    }

}