using Interfaces;
using UnityEngine;
using Utilities;

namespace Managers
{
    public static class Spawner
    {
        public static T Spawn<T>(T obj, Vector3 position, Quaternion rotation = default) where T: MonoBehaviour
        {
            var spawnedObj = Object.Instantiate(obj, position, rotation);
            CheckDependencies(ref spawnedObj);
            return spawnedObj;
        }

        public static GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
        {
            var spawnedObj = Object.Instantiate(obj, position, rotation);
            var behaviours = spawnedObj.GetComponents<MonoBehaviour>();
            for (var i = 0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                CheckDependencies(ref behaviour);
            }

            return spawnedObj;
        }

        private static void CheckDependencies<T>(ref T spawnedObj) where T : MonoBehaviour
        {
            ComponentInjector.InjectComponents(spawnedObj);
            var colorable = spawnedObj.GetComponent<IColorable>();
            if (colorable != null) ApplyColor(colorable);
        }

        private static void ApplyColor(IColorable colorable)
        {
            var color = GameManager.Instance.GetColor(colorable.Id);
            colorable.SetColor(color);
        }
    }
}