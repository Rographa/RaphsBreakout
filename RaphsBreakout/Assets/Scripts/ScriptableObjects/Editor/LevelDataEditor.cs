using System;
using System.Collections.Generic;
using Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using Random = UnityEngine.Random;

namespace ScriptableObjects.Editor
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        private LevelData _levelData;
        private Vector2 _scrollPosition;
        private CellType _selectedCellType = CellType.Brick;
        private bool _isDragging;
        private Dictionary<CellType, Texture2D> _textureCache;
        private float _cellSizeMultiplier = 40f;

        // Perlin Noise parameters
        private float _perlinScale = 5.0f;
        private float _perlinThreshold = 0.5f;
        
        // Random Fill parameters
        private float _fillPercentage = 0.3f;
        private float _clusterStrength = 0.05f;
        [Serializable] private enum SymmetryType {None, Horizontal, Vertical, Both}

        private SymmetryType _symmetry = SymmetryType.None;
        
        // Procedural Patterns parameters
        [Serializable] private enum PatternType { Stripes, Checkerboard, Columns }

        private PatternType _selectedPattern = PatternType.Stripes;
        private int _patternFrequency = 3;

        private void Awake()
        {
            _levelData = (LevelData)target;
            InitializeGrid();
            CacheTextures();
        }

        private void CacheTextures()
        {
            _textureCache = new Dictionary<CellType, Texture2D> {
                { CellType.None, CreateColorTexture(Color.gray) },
                { CellType.Brick, CreateColorTexture(Color.red) },
                { CellType.WallBrick , CreateColorTexture(Color.magenta)}
            };
        }

        private Texture2D CreateColorTexture(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            Vector2Int newSize = EditorGUILayout.Vector2IntField("Map Size", _levelData.MapSize);
            if (newSize != _levelData.MapSize)
            {
                _levelData.MapSize = newSize;
                InitializeGrid();
            }

            EditorGUILayout.Space();
            DrawPalette();
            EditorGUILayout.Space();
            
            var newCellSize = EditorGUILayout.FloatField("Cell Size Multiplier", _cellSizeMultiplier);

            if (newCellSize != _cellSizeMultiplier)
            {
                _cellSizeMultiplier = newCellSize;
            }
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
            DrawGrid();
            EditorGUILayout.EndScrollView();

            DrawSymmetryControls();
            DrawBulkTools();
            DrawPerlinNoiseTool();
            DrawRandomFillTool();
            DrawProceduralPatternTool();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_levelData);
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Force Save"))
            {
                EditorUtility.SetDirty(_levelData);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void DrawPalette()
        {
            EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            foreach (CellType type in Enum.GetValues(typeof(CellType)))
            {
                if (GUILayout.Toggle(_selectedCellType == type, 
                    new GUIContent(_textureCache[type]), 
                    "Button", 
                    GUILayout.Width(40), 
                    GUILayout.Height(40)))
                {
                    _selectedCellType = type;
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPerlinNoiseTool()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Perlin Noise", EditorStyles.boldLabel);
            _perlinScale = EditorGUILayout.Slider("Noise Scale", _perlinScale, 1f, 20f);
            _perlinThreshold = EditorGUILayout.Slider("Brick Threshold", _perlinThreshold, 0f, 1f);

            if (GUILayout.Button("Generate with Perlin Noise"))
            {
                GeneratePerlinNoise();
            }
        }

        private void GeneratePerlinNoise()
        {
            var width = _levelData.Grid.Count;
            var height = _levelData.Grid[0].list.Count;
            
            var offsetX = Random.Range(0f, 100f);
            var offsetY = Random.Range(0f, 100f);

            for (var x = 0; x < width; x++)
            {
                for (var y = _levelData.rowLimit; y < height; y++)
                {
                    if (!IsBaseCell(x, y, width, height)) continue;
                    var noiseValue = Mathf.PerlinNoise(x / _perlinScale + offsetX, y / _perlinScale + offsetY);
                    SetCellWithSymmetry(x,y, noiseValue > _perlinThreshold);
                }
            }
        }

        private void DrawSymmetryControls()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Symmetry", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            foreach (SymmetryType type in Enum.GetValues(typeof(SymmetryType)))
            {
                if (GUILayout.Toggle(_symmetry == type, 
                        new GUIContent(type.ToString()), 
                        "Button", 
                        GUILayout.Width(100), 
                        GUILayout.Height(20)))
                {
                    _symmetry = type;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

        private void UpdateSymmetry(SymmetryType type)
        {
            _symmetry = type;
        }

        private void DrawRandomFillTool()
        {
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Random Fill", EditorStyles.boldLabel);
            _fillPercentage = EditorGUILayout.Slider("Fill %", _fillPercentage, 0f, 1f);
            _clusterStrength = EditorGUILayout.Slider("Cluster Strength", _clusterStrength, 0f, 1f);
            if (GUILayout.Button("Random Fill"))
            {
                GenerateRandomFill();
            }
        }

        private void GenerateRandomFill()
        {
            var width = _levelData.Grid.Count;
            var height = _levelData.Grid[0].list.Count;

            for (var x = 0; x < width; x++)
            {
                for (var y = _levelData.rowLimit; y < height; y++)
                {
                    if (!IsBaseCell(x, y, width, height)) continue;

                    //var isBrick = Random.value < _fillPercentage;
                    var isBrick = CalculateByCluster(x, y);
                    SetCellWithSymmetry(x, y, isBrick);
                }
            }
        }

        private bool CalculateByCluster(int x, int y)
        {
            var bricksNearby = 0;
            var checkNorth = y - 1 >= 0;
            var checkWest = x - 1 >= 0;
            if (checkNorth && _levelData.Grid[x].list[y - 1] == (int)CellType.Brick)
            {
                bricksNearby++;
            }

            if (checkWest && _levelData.Grid[x - 1].list[y] == (int)CellType.Brick)
            {
                bricksNearby++;
            }

            if (checkNorth && checkWest && _levelData.Grid[x - 1].list[y - 1] == (int)CellType.Brick)
            {
                bricksNearby++;
            }

            return Random.value < _fillPercentage + (_clusterStrength * bricksNearby);
        }

        private bool IsBaseCell(int x, int y, int width, int height)
        {
            return _symmetry switch
            {
                SymmetryType.None => true,
                SymmetryType.Horizontal => y > (height - 1) / 2,
                SymmetryType.Vertical => x <= (width - 1) / 2,
                SymmetryType.Both => x <= (width - 1) / 2 && y > (height - 1) / 2,
                _ => true
            };
        }

        private void SetCellWithSymmetry(int x, int y, bool isBrick)
        {
            var mirrorX = _levelData.Grid.Count - x - 1;
            var mirrorY = _levelData.Grid[0].list.Count - y - 1;
            var value = isBrick ? (int)CellType.Brick : (int)CellType.None;
            var inRowLimit = mirrorY <= _levelData.rowLimit;

            _levelData.Grid[x].list[y] = value;

            switch (_symmetry)
            {
                case SymmetryType.None:
                    break;
                case SymmetryType.Horizontal:
                    if (!inRowLimit) _levelData.Grid[x].list[mirrorY] = value;
                    break;
                case SymmetryType.Vertical:
                    _levelData.Grid[mirrorX].list[y] = value;
                    break;
                case SymmetryType.Both:
                    _levelData.Grid[mirrorX].list[y] = value;
                    if (!inRowLimit)
                    {
                        _levelData.Grid[x].list[mirrorY] = value;
                        _levelData.Grid[mirrorX].list[mirrorY] = value;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawProceduralPatternTool()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Procedural Patterns", EditorStyles.boldLabel);
            _selectedPattern = (PatternType)EditorGUILayout.EnumPopup("Pattern", _selectedPattern);
            _patternFrequency = EditorGUILayout.IntField("Frequency", _patternFrequency);

            if (GUILayout.Button("Generate Pattern"))
            {
                GenerateProceduralPattern();
            }
        }

        private void GenerateProceduralPattern()
        {
            for (var x = 0; x < _levelData.Grid.Count; x++)
            {
                for (var y = _levelData.rowLimit; y < _levelData.Grid[0].list.Count; y++)
                {
                    var isBrick = _selectedPattern switch
                    {
                        PatternType.Stripes => (y % _patternFrequency == 0),
                        PatternType.Checkerboard => (x % _patternFrequency + y % _patternFrequency) % 2 == 0,
                        PatternType.Columns => (x % _patternFrequency == 0),
                        _ => false
                    };

                    _levelData.Grid[x].list[y] = isBrick ? (int)CellType.Brick : (int)CellType.None;
                }
            }
        }

        private void DrawGrid()
        {
            var cellMap = _levelData.GetCellMap();
            
            for (int y = cellMap.y - 1; y >= _levelData.rowLimit; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < cellMap.x; x++)
                {
                    var rect = GUILayoutUtility.GetRect(_cellSizeMultiplier * _levelData.CellSize.x, _cellSizeMultiplier * _levelData.CellSize.y);
                    GUI.DrawTexture(rect, _textureCache[(CellType)_levelData.Grid[x].list[y]]);

                    HandleCellInteraction(rect, x, y);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void HandleCellInteraction(Rect rect, int x, int y)
        {
            Event current = Event.current;
            var mouseRect = new Rect(current.mousePosition, _levelData.toolRadius);
            //if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
            if (current.type == EventType.MouseDown && mouseRect.Overlaps(rect))
            {
                _isDragging = true;
                SetCellWithSymmetry(x, y, _selectedCellType == CellType.Brick);
                //_levelData.Grid[x].list[y] = (int)_selectedCellType;
                current.Use();
            }
            //else if (_isDragging && current.type == EventType.MouseDrag && rect.Contains(current.mousePosition))
            else if (_isDragging && current.type == EventType.MouseDrag && mouseRect.Overlaps(rect))
            {
                SetCellWithSymmetry(x, y, _selectedCellType == CellType.Brick);
                //_levelData.Grid[x].list[y] = (int)_selectedCellType;
                current.Use();
            }
            else if (current.type == EventType.MouseUp)
            {
                _isDragging = false;
            }
        }

        private void DrawBulkTools()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill All"))
            {
                for (int x = 0; x < _levelData.Grid.Count; x++)
                    for (int y = _levelData.Grid[0].list.Count - 1; y >= _levelData.rowLimit; y--)
                        _levelData.Grid[x].list[y] = (int)_selectedCellType;
            }
            if (GUILayout.Button("Clear All"))
            {
                ClearAll();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Auto Wall Brick"))
            {
                for (int x = 0; x < _levelData.Grid.Count; x++)
                {
                    for (int y = 0; y < _levelData.Grid[0].list.Count; y++)
                    {
                        if (ShouldAddWallBrick(new(x, y)))
                        {
                            _levelData.Grid[x].list[y] = (int)CellType.WallBrick;
                        } else if (_levelData.Grid[x].list[y] == (int)CellType.WallBrick)
                        {
                            _levelData.Grid[x].list[y] = (int)CellType.None;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ClearAll()
        {
            for (int x = 0; x < _levelData.Grid.Count; x++)
                for (int y = _levelData.Grid[0].list.Count - 1; y >= 0; y--)
                    _levelData.Grid[x].list[y] = (int)CellType.None;
        }

        private void InitializeGrid()
        {
            var cellMapSize = _levelData.GetCellMap();
            if (_levelData.Grid == null || 
                _levelData.Grid.Count != cellMapSize.x || 
                _levelData.Grid[0].list.Count != cellMapSize.y)
            {
                _levelData.Grid = new List<ListWrapper>(cellMapSize.x);
                for (var i = 0; i < cellMapSize.x; i++)
                {
                    _levelData.Grid.Add(new(cellMapSize.y));
                    for (var j = 0; j < cellMapSize.y; j++)
                    {
                        _levelData.Grid[i].list.Add(0);
                    }
                }
                
            }
        }

        private bool ShouldAddWallBrick(Vector2Int position)
        {
            var cellMap = _levelData.GetCellMap();
            return position.x < _levelData.wallBorder ||
                   position.x > cellMap.x - _levelData.wallBorder - 1 ||
                   position.y < _levelData.wallBorder ||
                   position.y > cellMap.y - _levelData.wallBorder - 1;
        }
    }
}