using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Enums;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Design/Levels/Level Data", fileName = "LevelData")]
    public class LevelData : DataObject
    {
        private const string LevelDataPath = "Assets/Data/Levels";
        
        public uint levelId;
        [SerializeField] private int amountOfBalls = 3;
        [SerializeField] private Vector2Int mapSize = new Vector2Int(16, 10);
        [SerializeField] private Vector2 cellSize = new Vector2(0.5f, 0.25f);
        [SerializeField] private BrickData brickData;
        [Range(0, 40)] public int rowLimit = 8;
        [Range(1, 3)] public int wallBorder = 1;

        public Vector2 toolRadius;
        //public CellType[,] Grid;
        public List<ListWrapper> Grid;
        public int AmountOfBalls => amountOfBalls;

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

        [MenuItem("Breakout/Update Level IDs")]
        public static void GenerateLevelIDs()
        {

            var guidList = AssetDatabase.FindAssets($"t: {nameof(LevelData)}");
            foreach (var guid in guidList)
            {
                var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid));
                var result = new String(levelData.name.Where(Char.IsDigit).ToArray());
                var id = uint.Parse(result);
                levelData.levelId = id;
                EditorUtility.SetDirty(levelData);
            }
        }
    }

}