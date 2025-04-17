using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utilities
{
    public class ComponentInjector : MonoSingleton<ComponentInjector>
    {
        private static List<MonoBehaviour> _injected = new();
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
            if (_injected.Contains((MonoBehaviour)target)) return;
            var type = target.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<GetComponentAttribute>();
                if (attribute != null)
                {
                    switch (attribute)
                    {
                        case GetComponentsAttribute:
                            ApplyGetComponents(field, (MonoBehaviour)target, attribute.SearchChildren);
                            break;
                        default:
                            ApplyGetComponent(field, (MonoBehaviour)target, attribute.SearchChildren);
                            break;
                    }
                }
            }
            _injected.Add((MonoBehaviour)target);
        }

        private static void ApplyGetComponent(FieldInfo field, MonoBehaviour target, bool searchChildren)
        {
            
            var component = target.GetComponent(field.FieldType);
            if (component == null && searchChildren)
                component = target.GetComponentInChildren(field.FieldType);
            if (component != null)
            {
                field.SetValue(target, component);
            }
        }
        
        private static void ApplyGetComponents(FieldInfo field, MonoBehaviour target, bool searchChildren)
        {
            var type = field.FieldType;
            if (!type.IsArray) return;
            var elementType = type.GetElementType();

            var components = searchChildren switch
            {
                true => target.GetComponentsInChildren(elementType),
                false => target.GetComponents(elementType)
            };
            if (components != null)
            {
                var array = Array.CreateInstance(elementType, components.Length);
                Array.Copy(components, array, components.Length);
                field.SetValue(target, array);
                
            }
        }
    }
}