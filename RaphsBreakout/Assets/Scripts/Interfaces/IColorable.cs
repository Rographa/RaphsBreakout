using UnityEngine;

namespace Interfaces
{
    public interface IColorable
    {
        public ColorableId Id { get; set; }
        public void SetColor(Color color);
    }
}