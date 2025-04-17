using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class LevelSelectorController : MonoBehaviour
    {
        [GetComponents] private LevelUI[] _levelUIs;
        [SerializeField] private TextMeshProUGUI pageText;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button previousPageButton;
        [SerializeField] private int itemsPerPage = 15;
        [SerializeField] private float rawImageSize;
        

        [SerializeField] private List<LevelTexture> levelTextures = new();
        private Dictionary<ColorableId, Color> _cachedColors = new();
        private int _currentPage;
        private int _numberOfPages;

        private void Start()
        {
            ComponentInjector.InjectComponents(this);
            SetupColors();
            SetupPage();
            StartCoroutine(LoadLevelTextures(GameManager.Instance.Levels.ToArray()));
        }

        private void SetupPage()
        {
            var levelCount = GameManager.Instance.Levels.Count;
            _currentPage = 0;
            _numberOfPages = levelCount / itemsPerPage;
            if (levelCount % itemsPerPage != 0) _numberOfPages++;
            nextPageButton.onClick.AddListener(NextPage);
            previousPageButton.onClick.AddListener(PreviousPage);
        }

        private void NextPage()
        {
            _currentPage = Mathf.Clamp(_currentPage + 1, 0, _numberOfPages - 1);
            UpdatePage();
        }
        
        private void PreviousPage()
        {
            _currentPage = Mathf.Clamp(_currentPage - 1, 0, _numberOfPages - 1);
            UpdatePage();
        }

        private void UpdatePage()
        {
            for (var i = 0; i < itemsPerPage; i++)
            {
                var levelTextureIndex = i + (itemsPerPage * _currentPage);
                var hasLevelTexture = levelTextureIndex < levelTextures.Count;
                _levelUIs[i].gameObject.SetActive(hasLevelTexture);
                if (!hasLevelTexture) continue;
                _levelUIs[i].SetLevel(levelTextures[levelTextureIndex].texture, levelTextures[levelTextureIndex].level);
            }

            pageText.text = (_currentPage + 1).ToString();
        }

        private void SetupColors()
        {
            foreach (ColorableId id in Enum.GetValues(typeof(ColorableId)))
            {
                _cachedColors.Add(id, GameManager.Instance.GetColor(id));
            }
        }
        private void SetLevelUISize(Vector2 size)
        {
            foreach (var levelUI in _levelUIs)
            {
                levelUI.SetSize(size);
            }
        }
        
        private IEnumerator LoadLevelTextures(LevelData[] levels)
        {
            SetLevelUISize(new(levels.First().MapSize.x * rawImageSize, levels.First().MapSize.y * rawImageSize));
            foreach (var level in levels)
            {
                var cellMap = level.GetCellMap();
                var texture = new Texture2D(cellMap.x, cellMap.y);
                ThreadedRequester.Request(() => CreateLevelColorMap(level, ref texture), OnTextureCreated);    
            }
            yield return new WaitUntil(() => levelTextures.Count == levels.Length);
            levelTextures = levelTextures.OrderBy(x => x.level.levelId).ToList();
            UpdatePage();
        }
        
        private LevelTexture CreateLevelColorMap(LevelData level, ref Texture2D texture)
        {
            List<Color> colorMap = new List<Color>(texture.width * texture.height);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var color = (CellType)level.Grid[x].list[y] switch
                    {
                        CellType.None => _cachedColors[ColorableId.Background],
                        CellType.Brick => _cachedColors[ColorableId.StrongBrick],
                        CellType.WallBrick => _cachedColors[ColorableId.WallBrick],
                        _ => Color.white
                    };
                    colorMap.Add(color);
                }
            }
            return new (level, texture, colorMap.ToArray());
        }
        
        private void OnTextureCreated(object texObj)
        {
            var result = (LevelTexture)texObj;
            result.texture.SetPixels(result.colorMap);
            result.texture.filterMode = FilterMode.Point;
            result.texture.Apply();
            levelTextures.Add(result);
        }
    }
    
    [Serializable]
    public struct LevelTexture
    {
        public LevelData level;
        public Texture2D texture;
        public Color[] colorMap;

        public LevelTexture(LevelData level, Texture2D texture, Color[] colorMap)
        {
            this.level = level;
            this.texture = texture;
            this.colorMap = colorMap;
        }
    }
}
