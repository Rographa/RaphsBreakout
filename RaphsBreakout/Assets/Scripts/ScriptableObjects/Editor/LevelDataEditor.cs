using System;
using System.Collections.Generic;
using Enums;
using UnityEditor;
using UnityEngine;
using Utilities;

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
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
            DrawGrid();
            EditorGUILayout.EndScrollView();

            DrawBulkTools();

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

        private void DrawGrid()
        {
            var cellMap = _levelData.GetCellMap();
            for (int y = cellMap.y - 1; y >= _levelData.rowLimit; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < cellMap.x; x++)
                {
                    var rect = GUILayoutUtility.GetRect(20 * _levelData.CellSize.x, 20 * _levelData.CellSize.y);
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
                _levelData.Grid[x].list[y] = (int)_selectedCellType;
                current.Use();
            }
            //else if (_isDragging && current.type == EventType.MouseDrag && rect.Contains(current.mousePosition))
            else if (_isDragging && current.type == EventType.MouseDrag && mouseRect.Overlaps(rect))
            {
                _levelData.Grid[x].list[y] = (int)_selectedCellType;
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
                for (int x = 0; x < _levelData.Grid.Count; x++)
                    for (int y = _levelData.Grid[0].list.Count - 1; y >= 0; y--)
                        _levelData.Grid[x].list[y] = (int)CellType.None;
            }
            EditorGUILayout.EndHorizontal();
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
    }
}