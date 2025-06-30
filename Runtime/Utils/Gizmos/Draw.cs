using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDevForBeginners
{
    public enum ConsumeType
    {
        None,
        Update,
        LateUpdate,
        FixedUpdate
    }
    
    public enum GizmoCommandType
    {
        None,
        DrawLine,
        DrawSphere,
        DrawBox,
        DrawCapsule,
        DrawPlane,
        DrawCylinder
    }

    public struct DrawCommand
    {
        public ConsumeType consumeType;
        public GizmoCommandType gizmoCommandType;
        public Matrix4x4 matrix;

        public void Execute(DrawResources drawResources)
        {
            switch (gizmoCommandType)
            {
                case GizmoCommandType.DrawBox:
                    Graphics.DrawMesh(drawResources.boxMesh, matrix, drawResources.defaultMaterial, 0);
                    break;
                case GizmoCommandType.DrawLine:
                    break;
                case GizmoCommandType.DrawSphere:
                    Graphics.DrawMesh(drawResources.sphereMesh, matrix, drawResources.defaultMaterial, 0);
                    break;
                case GizmoCommandType.DrawCapsule:
                    Graphics.DrawMesh(drawResources.capsuleMesh, matrix, drawResources.defaultMaterial, 0);
                    break;
                case GizmoCommandType.DrawPlane:
                    Graphics.DrawMesh(drawResources.planeMesh, matrix, drawResources.defaultMaterial, 0);
                    Debug.Log("Draw Plane");
                    break;
                case GizmoCommandType.DrawCylinder:
                    Graphics.DrawMesh(drawResources.cylinderMesh, matrix, drawResources.defaultMaterial, 0);
                    break;
            }
        }
    }

    public class DrawResources
    {
        public Mesh boxMesh { get; private set; }
        public Mesh sphereMesh { get; private set; }
        public Mesh capsuleMesh { get; private set; }
        public Mesh cylinderMesh { get; private set; }
        public Mesh planeMesh { get; private set; }

        public Material defaultMaterial { get; private set; }

        public void Init()
        {
            boxMesh = GetMesh(PrimitiveType.Cube);
            sphereMesh = GetMesh(PrimitiveType.Sphere);
            capsuleMesh = GetMesh(PrimitiveType.Capsule);
            cylinderMesh = GetMesh(PrimitiveType.Cylinder);
            planeMesh = GetMesh(PrimitiveType.Plane);

            defaultMaterial = GetMaterial();
        }

        static Material GetMaterial()
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Material material = Object.Instantiate(go.GetComponent<MeshRenderer>().sharedMaterial);
            GameObject.Destroy(go);
            return material;
        }

        static Mesh GetMesh(PrimitiveType primitiveType)
        {
            GameObject go = GameObject.CreatePrimitive(primitiveType);
            Mesh mesh = Object.Instantiate(go.GetComponent<MeshFilter>().sharedMesh);
            GameObject.Destroy(go);
            return mesh;
        }
    }

    [DefaultExecutionOrder(-10000)]
    public class Draw : MonoBehaviour
    {
        private static Draw _instance = null;
        public static bool exists => _instance != null;

        private Dictionary<string, DrawCommand> _drawCommands = new Dictionary<string, DrawCommand>();
        private DrawResources _drawResources = new DrawResources();
        
        public static Draw GetOrCreate()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject(nameof(Draw));
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<Draw>();
                _instance._drawResources.Init();
            }

            return _instance;
        }

        public static void Line(string name, ConsumeType consumeType, Vector3 a, Vector3 b)
        {
            Draw gizmos = GetOrCreate();
            gizmos._drawCommands[name] = new DrawCommand()
            {
                consumeType = consumeType,
                gizmoCommandType = GizmoCommandType.DrawLine
            };
        }

        public static void Box(string name, ConsumeType consumeType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Draw gizmos = GetOrCreate();
            gizmos._drawCommands[name] = new DrawCommand()
            {
                consumeType = consumeType,
                gizmoCommandType = GizmoCommandType.DrawLine,
                matrix = Matrix4x4.TRS(position, rotation, scale)
            };
        }

        public static void Sphere(string name, ConsumeType consumeType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Draw gizmos = GetOrCreate();
            gizmos._drawCommands[name] = new DrawCommand()
            {
                consumeType = consumeType,
                gizmoCommandType = GizmoCommandType.DrawSphere,
                matrix = Matrix4x4.TRS(position, rotation, scale)
            };
        }

        public static void Cylinder(string name, ConsumeType consumeType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Draw gizmos = GetOrCreate();
            gizmos._drawCommands[name] = new DrawCommand()
            {
                consumeType = consumeType,
                gizmoCommandType = GizmoCommandType.DrawCylinder,
                matrix = Matrix4x4.TRS(position, rotation, scale)
            };
        }

        public static void Capsule(string name, ConsumeType consumeType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Draw gizmos = GetOrCreate();
            gizmos._drawCommands[name] = new DrawCommand()
            {
                consumeType = consumeType,
                gizmoCommandType = GizmoCommandType.DrawCapsule,
                matrix = Matrix4x4.TRS(position, rotation, scale)
            };
        }

        public static void Plane(string name, ConsumeType consumeType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Draw gizmos = GetOrCreate();
            gizmos._drawCommands[name] = new DrawCommand()
            {
                consumeType = consumeType,
                gizmoCommandType = GizmoCommandType.DrawPlane,
                matrix = Matrix4x4.TRS(position, rotation, scale)
            };
        }

        private void FixedUpdate()
        {
            ConsumeList(ConsumeType.FixedUpdate);
        }

        private void Update()
        {
            ConsumeList(ConsumeType.Update);
        }
        
        private void LateUpdate()
        {
            ExecuteCommandList();
            ConsumeList(ConsumeType.LateUpdate);
        }

        private void OnDisable()
        {
            _drawCommands.Clear();
        }

        void ExecuteCommandList()
        {
            foreach (var valueKeyPair in _drawCommands)
            {
                valueKeyPair.Value.Execute(_drawResources);
            }
        }

        void ConsumeList(ConsumeType consumeType)
        {
            List<string> consumeKeys = new List<string>();
            foreach (var valueKeyPair in _drawCommands)
            {
                if (consumeType == valueKeyPair.Value.consumeType)
                {
                    consumeKeys.Add(valueKeyPair.Key);
                }
            }

            foreach (var consumeKey in consumeKeys)
            {
                _drawCommands.Remove(consumeKey);
            }
        }
    }
}