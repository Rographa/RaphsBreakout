using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Gameplay;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] private LevelData level;
        [FormerlySerializedAs("wallBlockPrefab")] [SerializeField] private GameObject wallBrickPrefab;
        [SerializeField] private Ball ballPrefab;
        [GetComponent] private Grid _grid;

        private int _width;
        private int _height;
        private Camera _mainCamera;

        private List<GameObject> _wallBrickList = new();
        private PolygonCollider2D _wallBrickCollider;
        
        private IEnumerator Start()
        {
            _mainCamera = Camera.main;
            transform.position = CameraManager.Main.ViewportToWorldPoint(Vector3.zero);
            SetupGrid();
            yield return new WaitForSeconds(0.2f);
            SetupLevel();
            yield return new WaitForSeconds(0.2f);
            SpawnBall();
        }

        private void SetupGrid()
        {
            var screenWidth = _mainCamera.ViewportToWorldPoint(Vector2.right).x -
                              _mainCamera.ViewportToWorldPoint(Vector2.zero).x;
            var screenHeight = _mainCamera.ViewportToWorldPoint(Vector2.up).y -
                               _mainCamera.ViewportToWorldPoint(Vector2.zero).y;
            _width = Mathf.CeilToInt(screenWidth / _grid.cellSize.x);
            _height = Mathf.CeilToInt(screenHeight / _grid.cellSize.y);

            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    var pos = new Vector2Int(j, i);
                    if (ShouldAddWallBrick(pos))
                    {
                        var wallBrick = SpawnWallBrick(pos);
                        wallBrick.transform.parent = transform;
                        _wallBrickList.Add(wallBrick);
                    }
                }
            }

            _wallBrickCollider = gameObject.AddComponent<PolygonCollider2D>();
            _wallBrickCollider.offset = transform.InverseTransformPoint(Vector3.zero);
            var list = new List<Bounds>();
            foreach (var wallBrick in _wallBrickList)
            {
                var box = wallBrick.GetComponent<BoxCollider2D>();
                list.Add(box.bounds);
            }

            _wallBrickCollider.pathCount = list.Count;
            for (var i = 0; i < list.Count; i ++)
            {
                
                var bounds = list[i];
                Vector2 point1 = bounds.min;
                Vector2 point2 = new Vector2(bounds.min.x, bounds.max.y);
                Vector2 point3 = bounds.max;
                Vector2 point4 = new Vector2(bounds.max.x, bounds.min.y);
                
                _wallBrickCollider.SetPath(i, new []{point1, point2, point3, point4});
            }
        }

        private void SetupLevel()
        {
            for (var i = 0; i < level.Grid.Count; i++)
            {
                for (var j = 0; j < level.Grid[0].list.Count; j++)
                {
                    ProcessCell((CellType)level.Grid[i].list[j], i, j);
                }
            }
        }

        private void ProcessCell(CellType cellType, int x, int y)
        {
            var pos = _grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
            switch (cellType)
            {
                case CellType.None:
                    break;
                case CellType.Brick:
                    level.BrickData.Spawn(pos);
                    break;
                case CellType.WallBrick:
                    SpawnWallBrick(pos);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
            }
        }

        private GameObject SpawnWallBrick(Vector2Int pos)
        {
            var point = _grid.GetCellCenterWorld((Vector3Int)pos);
            point.z = 0;
            return SpawnWallBrick(point);
        }
        
        private GameObject SpawnWallBrick(Vector2 pos)
        {
            return Spawner.Spawn(wallBrickPrefab, pos, Quaternion.identity);
        }

        private void SpawnBall(Vector2 pos = default)
        {
            var ball = Spawner.Spawn(ballPrefab, pos, Quaternion.identity);
            var data = GameManager.Instance.GetBallData();
            ball.SetupBall(data);
        }

        private bool ShouldAddWallBrick(Vector2Int position)
        {
            return position.x == 0 || position.x == _width - 2 || position.y == 0 || position.y == _height - 1;
        }
    }
}
