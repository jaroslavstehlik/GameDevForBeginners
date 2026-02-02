using System;
using UnityEngine;

namespace GameDevForBeginners
{
    public class RectCastDescriptor : MonoBehaviour
    {
        public Vector2 offset = new Vector2(0.0f, -0.25f);
        public Vector2 size = new Vector2(0.5f, 0.5f);
        public float distance = 0f;
        public bool isColliding = false;
        public bool drawDebug = true;
        public float angle => transform.rotation.eulerAngles.z;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Color _collidingColor = Color.red;

        public Vector2 basePosition => transform.position;
        public float baseRotation => transform.rotation.eulerAngles.z;
        public Vector2 rayOrigin => transform.TransformPoint(offset);
        public Vector2 rayDirection => transform.up;
 
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!drawDebug)
                return;
            
            DrawWireRect(rayOrigin, rayDirection, size, distance, isColliding ? _collidingColor : _color);
        }

        public static void DrawWireRect(Vector2 _pos, Vector2 direction, Vector2 size, float distance,
            Color _color = default(Color))
        {
            if (_color != default(Color))
                UnityEditor.Handles.color = _color;

            Vector2 halfSize = size * 0.5f;

            Vector2 up = direction;
            Vector2 right = Vector3.Cross(up, Vector3.forward);

            Vector3 v0 = _pos - right * halfSize.x - up * halfSize.y;
            Vector3 v1 = _pos + right * halfSize.x - up * halfSize.y;
            Vector3 v2 = _pos + up * distance - right * halfSize.x + up * halfSize.y;
            Vector3 v3 = _pos + up * distance + right * halfSize.x + up * halfSize.y;

            UnityEditor.Handles.DrawLine(v0, v1);            
            UnityEditor.Handles.DrawLine(v2, v3);
            UnityEditor.Handles.DrawLine(v0, v2);
            UnityEditor.Handles.DrawLine(v1, v3);
        }
#endif
    }
}