using System;
using UnityEngine;

namespace Utilities
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    if (!_instance._isInitialized) _instance.Init();
                    return _instance;
                }
                _instance = FindFirstObjectByType<T>();
                return _instance;
            }
        }

        private bool _isInitialized;

        protected virtual void Init()
        {
            _isInitialized = true;
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            if (!_isInitialized)
            {
                Init();
            }
        }
    }
}
