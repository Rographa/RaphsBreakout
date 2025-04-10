using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkit
{
    public class LevelDataCustomEditor : EditorWindow
    {
        [SerializeField] private LevelData level;
    
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Window/UI Toolkit/LevelDataCustomEditor")]
        public static void ShowExample()
        {
            LevelDataCustomEditor wnd = GetWindow<LevelDataCustomEditor>();
            wnd.titleContent = new GUIContent("LevelDataCustomEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            if (level != null)
            {
                var levelHolder = new VisualElement();
                var mapSize = level.GetCellMap();
                
                for (var i = mapSize.y; i > mapSize.y/2; i--)
                {
                    var line = new ToggleButtonGroup(string.Empty)
                            { isMultipleSelection = true, allowEmptySelection = true };
                    for (var j = 0; j < mapSize.x; j++)
                    {
                        line.Add(new Button() { text = " ", tooltip = "Has Brick?" });
                    }
                    levelHolder.Add(line);
                }
                root.Add(levelHolder);
            }

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
        }
    }
}
