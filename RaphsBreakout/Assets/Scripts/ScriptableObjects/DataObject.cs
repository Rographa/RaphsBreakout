using UnityEngine;

namespace ScriptableObjects
{
    public class DataObject : ScriptableObject
    {
        
    }

    public static class DataObjectExtensions
    {
        public static T Clone<T>(this T obj) where T : DataObject
        {
            return Object.Instantiate(obj) as T;
        }
    }
}