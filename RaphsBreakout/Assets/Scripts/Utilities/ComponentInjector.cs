using System;
using System.Reflection;
using UnityEngine;

namespace Utilities
{
    public class ComponentInjector : MonoBehaviour
    {
        private void Awake()
        {
            InjectAll();
        }

        private static void InjectAll()
        {
            var allScripts =
                FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            foreach (var script in allScripts)
            {
                InjectComponents(script);
            }
        }

        public static void InjectComponents(object target)
        {
            var type = target.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<GetComponentAttribute>(); 
                if (attribute == null) continue;
                var component = ((MonoBehaviour)target).GetComponent(field.FieldType);
                if (component == null && attribute.SearchChildren)
                    component = ((MonoBehaviour)target).GetComponentInChildren(field.FieldType);
                if (component != null)
                {
                    field.SetValue(target, component);
                }
            }
        }
    }
}