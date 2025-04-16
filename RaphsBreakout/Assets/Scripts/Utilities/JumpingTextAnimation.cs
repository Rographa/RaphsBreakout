using System;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public class JumpingTextAnimation : MonoBehaviour
    {
        [SerializeField] private float amplitude = 10f;
        [SerializeField] private float frequency = 2f;
        [SerializeField] private float charDelay = 0.1f;

        [GetComponent] private TMP_Text _textComponent;
        private TMP_TextInfo _textInfo;
        private Vector3[][] _originalVertices;

        private void Start()
        {
            ComponentInjector.InjectComponents(this);
            CacheOriginalVertices();
        }

        private void CacheOriginalVertices()
        {
            _textComponent.ForceMeshUpdate();
            _textInfo = _textComponent.textInfo;
            _originalVertices = new Vector3[_textInfo.characterCount][];

            for (var i = 0; i < _textInfo.characterCount; i++)
            {
                var charInfo = _textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                var vertices = _textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                _originalVertices[i] = new Vector3[4];
                Array.Copy(vertices, charInfo.vertexIndex, _originalVertices[i], 0, 4);
            }
        }

        private void Update()
        {
            AnimateCharacters();
        }

        private void AnimateCharacters()
        {
            _textComponent.ForceMeshUpdate();
            var meshInfo = _textInfo.meshInfo;

            for (var i = 0; i < _textInfo.characterCount; i++)
            {
                var charInfo = _textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                var offset = Mathf.Sin(Time.time * frequency + i * charDelay) * amplitude;
                ApplyOffsetToCharacter(i, offset, ref meshInfo[charInfo.materialReferenceIndex].vertices);
            }

            for (var i = 0; i < meshInfo.Length; i++)
            {
                meshInfo[i].mesh.vertices = meshInfo[i].vertices;
                _textComponent.UpdateGeometry(meshInfo[i].mesh, i);
            }
        }

        private void ApplyOffsetToCharacter(int charIndex, float offset, ref Vector3[] vertices)
        {
            var vertexIndex = _textInfo.characterInfo[charIndex].vertexIndex;

            for (var j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = _originalVertices[charIndex][j] + new Vector3(0, offset, 0);
            }
        }
    }
}