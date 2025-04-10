using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class ColorSettingsData : DataObject
    {
        public List<ColorData> colorDataList;
        public List<string> colorableIds;

        public Color Get(ColorableId id) => colorDataList.First(x => x.id == id).color;
    }

    [Serializable]
    public class ColorData
    {
        public ColorableId id;
        public Color color;
    }
}