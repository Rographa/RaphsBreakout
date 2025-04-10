using System;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Utilities.Editor
{
    [CustomEditor(typeof(ColorSettingsData))]
    public class ColorSettingsDataEditor : UnityEditor.Editor
    {
        private ColorSettingsData _colorSettings;
        private string _enumFilePath = "Assets/Scripts/Enums/";
        private string _enumFileName = "ColorableId";

        private void OnValidate()
        {
            _colorSettings ??= (ColorSettingsData)target;
        }

        public override void OnInspectorGUI()
        {
            _colorSettings ??= (ColorSettingsData)target;
            base.OnInspectorGUI();

            _enumFilePath = EditorGUILayout.TextField("Path", _enumFilePath);
            _enumFileName = EditorGUILayout.TextField("Name", _enumFileName);

            if (GUILayout.Button("Save"))
            {
                EditorMethods.WriteToEnum(_enumFilePath, _enumFileName, _colorSettings.colorableIds);
            }
        }
    }
}