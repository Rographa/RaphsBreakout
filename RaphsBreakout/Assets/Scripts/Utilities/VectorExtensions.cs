using UnityEngine;

namespace Utilities
{
    public static class VectorExtensions
    {
        public static Vector3 ClampMagnitude(this Vector3 vector, float maxMagnitude)
        {
            var sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > maxMagnitude * maxMagnitude)
            {
                return vector.normalized * maxMagnitude;
            }

            return vector;
        }
        
        public static Vector2 ClampMagnitude(this Vector2 vector, float maxMagnitude)
        {
            var sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > maxMagnitude * maxMagnitude)
            {
                return vector.normalized * maxMagnitude;
            }

            return vector;
        }
    }
}