using System;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class LevelUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject selected;
        [GetComponent] private RawImage _rawImage;
        [GetComponent] private Button _button;

        private LevelData _level;
        private void Start()
        {
            ComponentInjector.InjectComponents(this);
            _button.onClick.AddListener(LoadLevel);
        }

        public void SetLevel(Texture2D texture, LevelData level)
        {
            _level = level;
            _rawImage.texture = texture;
        }

        private void LoadLevel()
        {
            GameManager.LoadLevel(_level);
        }

        public void SetSize(Vector2 size)
        {
            ((RectTransform)transform).sizeDelta = size;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _button.Select();
            selected.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            selected.SetActive(false);
        }
    }
}
