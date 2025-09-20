using System;
using UnityEngine;

namespace GameDevForBeginners
{
    public class SphereCast : MonoBehaviour
    {
        public float radius = 0.5f;
        public float height = 1f;
        public bool isColliding = false;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Color _collidingColor = Color.red;
 
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            DrawWireCapsule(position + rotation * Vector3.up * height * 0.5f, rotation, radius, height,
                isColliding ? _collidingColor : _color);
        }

        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height,
            Color _color = default(Color))
        {
            if (_color != default(Color))
                UnityEditor.Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, UnityEditor.Handles.matrix.lossyScale);
            using (new UnityEditor.Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = _height / 2f;

                //draw sideways
                UnityEditor.Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                UnityEditor.Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                UnityEditor.Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                UnityEditor.Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                UnityEditor.Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                UnityEditor.Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                UnityEditor.Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                UnityEditor.Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                UnityEditor.Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                UnityEditor.Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
            }
        }
#endif
    }
}