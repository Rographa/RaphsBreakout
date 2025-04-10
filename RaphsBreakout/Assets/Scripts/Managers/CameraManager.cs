using Interfaces;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CameraManager : MonoSingleton<CameraManager>, IColorable
    {
        public static Camera Main
        {
            get
            {
                if (_main != null) return _main;
                _main = Camera.main;
                return _main;
            }
        }

        private static Camera _main;
        [field: SerializeField] public ColorableId Id { get; set; }
        public void SetColor(Color color)
        {
            Main.backgroundColor = color;
        }
    }
}
