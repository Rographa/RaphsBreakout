using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Gameplay;
using Gameplay.PowerUpEffects;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] private LevelData level;
        [SerializeField] private Vector3 ballInitialWorldPos;
        [FormerlySerializedAs("wallBlockPrefab")] [SerializeField] private GameObject wallBrickPrefab;
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private PowerUp powerUpPrefab;
        [GetComponent] private Grid _grid;

        private int _width;
        private int _height;
        private Camera _mainCamera;

        private List<GameObject> _wallBrickList = new();
        private PolygonCollider2D _wallBrickCollider;
        private List<Ball> _activeBalls = new();

        public Paddle Paddle
        {
            get
            {
                if (_paddle != null) return _paddle;
                _paddle = FindFirstObjectByType<Paddle>();
                return _paddle;
            }
        }

        private Paddle _paddle;
        
        private IEnumerator Start()
        {
            _mainCamera = Camera.main;
            transform.position = CameraManager.Main.ViewportToWorldPoint(Vector3.zero);
            SetupGrid();
            yield return new WaitForSeconds(0.2f);
            SetupLevel();
            yield return new WaitForSeconds(0.2f);
            
            //Removing it for now
            /*
            for (var i = 0; i < level.AmountOfBalls; i++)
            {
                SpawnBall_Internal(ballInitialWorldPos);
                yield return new WaitForSeconds(0.2f);
            }*/
        }

        private void SetupGrid()
        {
            var screenWidth = _mainCamera.ViewportToWorldPoint(Vector2.right).x -
                              _mainCamera.ViewportToWorldPoint(Vector2.zero).x;
            var screenHeight = _mainCamera.ViewportToWorldPoint(Vector2.up).y -
                               _mainCamera.ViewportToWorldPoint(Vector2.zero).y;
            _width = Mathf.CeilToInt(screenWidth / _grid.cellSize.x);
            _height = Mathf.CeilToInt(screenHeight / _grid.cellSize.y);

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
                    var wallBrick = SpawnWallBrick(pos);
                    wallBrick.transform.parent = transform;
                    _wallBrickList.Add(wallBrick);
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

        private Ball SpawnBall_Internal(Vector2 pos = default, bool spawnInPaddle = false)
        {
            var ball = Spawner.Spawn(ballPrefab, pos, Quaternion.identity);
            var data = GameManager.Instance.GetBallData();
            ball.SetupBall(data, spawnInPaddle);
            _activeBalls.Add(ball);
            return ball;
        }

        private void DestroyBall_Internal(Ball ball)
        {
            _activeBalls.Remove(ball);
            Destroy(ball.gameObject);
        }

        public static Ball SpawnBall(Vector2 pos = default)
        {
            return Instance.SpawnBall_Internal(pos);
        }

        public static Ball SpawnBallInPaddle(Vector2 pos)
        {
            return Instance.SpawnBall_Internal(pos, true);
        }

        public static void DestroyBall(Ball ball)
        {
            Instance.DestroyBall_Internal(ball);
        }

        public static void ApplyPowerUp(PowerUpData data)
        {
            Instance.ApplyPowerUp_Internal(data);
        }

        private void ApplyPowerUp_Internal(PowerUpData data)
        {
            foreach (var effect in data.Effects)
            {
                switch (effect.Target)
                {
                    case EffectTarget.Global:
                        ApplyGlobalEffect(effect);
                        break;
                    case EffectTarget.Paddle:
                        ApplyPaddleEffect(effect);
                        break;
                    case EffectTarget.SingleBall:
                        ApplySingleBallEffect(effect);
                        break;
                    case EffectTarget.AllBalls:
                        ApplyAllBallsEffect(effect);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void ApplyGlobalEffect(PowerUpEffect effect){}

        private void ApplyPaddleEffect(PowerUpEffect effect)
        {
            effect.Apply(Paddle.gameObject);
        }

        private void ApplySingleBallEffect(PowerUpEffect effect)
        {
            effect.Apply(_activeBalls.FirstOrDefault().gameObject);
        }

        private void ApplyAllBallsEffect(PowerUpEffect effect)
        {
            foreach (var ball in _activeBalls)
            {
                effect.Apply(ball.gameObject);
            }
        }

        public static void RequestPowerUp(Vector3 pos)
        {
            var powerUpData = GameManager.Instance.PowerUpSettings.GetRandom();
            var powerUp = Spawner.Spawn(Instance.powerUpPrefab, pos);
            powerUp.Setup(powerUpData);
        }
    }
}
