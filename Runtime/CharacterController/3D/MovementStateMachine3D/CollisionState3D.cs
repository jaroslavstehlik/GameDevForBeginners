using System;
using UnityEngine;

namespace GameDevForBeginners
{
    public struct SphereCastInfo
    {
        public RaycastHit[] raycastHits;
        internal int closestNormalRaycastHitIndex;
        internal int closestDistanceRaycastHitIndex;

        public Rigidbody GetRigidbody()
        {
            foreach (var raycastHit in raycastHits)
            {
                if (raycastHit.rigidbody != null)
                {
                    return raycastHit.rigidbody;
                }
            }

            return null;
        }
        
        public bool collides
        {
            get
            {
                return closestDistanceRaycastHitIndex != -1;
            }
        }

        public bool closestNormalRaycastHit(out RaycastHit raycastHit)
        {
            if (raycastHits == null ||
                closestNormalRaycastHitIndex < 0 ||
                closestNormalRaycastHitIndex >= raycastHits.Length)
            {
                raycastHit = new RaycastHit();
                return false;
            }

            raycastHit = raycastHits[closestNormalRaycastHitIndex]; 
            return true;
        }
        
        public bool closestDistanceRaycastHit(out RaycastHit raycastHit){
            if (raycastHits == null ||
                closestDistanceRaycastHitIndex < 0 ||
                closestDistanceRaycastHitIndex >= raycastHits.Length)
            {
                raycastHit = new RaycastHit();
                return false;
            }

            raycastHit = raycastHits[closestDistanceRaycastHitIndex];
            return true;
        }
        
        public Ray ray;
        public float radius;
        public float castDistance;

        public void DebugDrawHits()
        {
#if UNITY_EDITOR
            if(raycastHits == null)
                return;
            for (int i = 0; i < raycastHits.Length; i++)
            {
                Gizmos.DrawWireSphere(raycastHits[i].point, 0.01f);
            }
#endif
        }

        public void DebugDrawNormals(float length = 1f)
        {
#if UNITY_EDITOR
            if(raycastHits == null)
                return;
            for (int i = 0; i < raycastHits.Length; i++)
            {
                Gizmos.DrawLine(raycastHits[i].point, raycastHits[i].point + raycastHits[i].normal * length);
            }
#endif
        }
    }

    public class CollisionInfo
    {
        public Vector3 normal { get; private set; }
        public float rampDistance { get; private set; }
        public float rampLocalAngle { get; private set; }
        public float projectedRampDistance { get; private set; }

        public CollisionInfo(Vector3 up, Vector3 basePosition, Quaternion baseRotation, float radius, RaycastHit closestDistanceRaycastHit)
        {
            Vector3 pointDirection = closestDistanceRaycastHit.point - basePosition;
            float distanceToSurface = pointDirection.magnitude - radius;
            normal = -pointDirection.normalized;
            rampDistance = distanceToSurface;
            rampLocalAngle = Vector3.Angle(normal, up);            

            Vector3 baseUp = baseRotation * Vector3.up;            
            Vector3 closestPointDirectionNormalized = (closestDistanceRaycastHit.point - basePosition).normalized;
            Vector3 closestPointOnSurface = basePosition + closestPointDirectionNormalized * radius;
            projectedRampDistance = Vector3.Dot(baseUp, closestDistanceRaycastHit.point - closestPointOnSurface);
        }
    }
    
    public struct CollisionStateInfo
    {
        public Vector3 basePosition { get; private set; }
        public Quaternion baseRotation { get; private set; }
        public float maxSlopeAngle { get; private set; }
        public CollisionInfo collisionInfo { get; private set; }
        public SphereCastInfo sphereCastInfo;
        public Matrix4x4 localToWorld => Matrix4x4.TRS(basePosition, baseRotation, Vector3.one);
        public Matrix4x4 worldToLocal => localToWorld.inverse;
        public Vector3 up => -Physics.gravity.normalized;
        public Vector3 localUp => worldToLocal.MultiplyVector(up);

        public CollisionStateInfo(SphereCastDescriptor groundDetector, float maxSlopeAngle, LayerMask environmentMask)
        {
            this.collisionInfo = null;
            this.maxSlopeAngle = maxSlopeAngle;
            this.basePosition = groundDetector.basePosition;
            this.baseRotation = groundDetector.baseRotation;

            Ray groundRay = new Ray(groundDetector.rayOrigin, groundDetector.rayDirection);
            CollisionState3D.SphereCast(out sphereCastInfo, groundRay, groundDetector.basePosition, groundDetector.height, groundDetector.radius,
                environmentMask, maxSlopeAngle);

            groundDetector.isColliding = sphereCastInfo.collides;
            
            bool touching = true;
            touching &= sphereCastInfo.closestDistanceRaycastHit(out RaycastHit closestDistanceRaycastHit);
            touching &= sphereCastInfo.closestNormalRaycastHit(out RaycastHit closestNormalRaycastHit);
            if (touching)
            {
                collisionInfo = new CollisionInfo(up, basePosition, baseRotation, groundDetector.radius, closestDistanceRaycastHit);
            }
        }
    }

    public class CollisionState3D : MonoBehaviour
    {
        [SerializeField] private SphereCastDescriptor _groundDetector;
        [SerializeField] private SphereCastDescriptor _ceilingDetector;
        public LayerMask environmentMask = int.MaxValue;
        
        public CollisionStateInfo GetGroundStateInfo(float maxSlopeAngle)
        {
            return new CollisionStateInfo(_groundDetector, maxSlopeAngle, environmentMask);
        }
        
        public CollisionStateInfo GetCeilingStateInfo()
        {
            return new CollisionStateInfo(_ceilingDetector, float.MaxValue, environmentMask);
        }

        public static void SphereCast(out SphereCastInfo sphereCastInfo, Ray ray, Vector3 basePosition, float castDistance, float radius, int layerMask, float maxAngle)
        {
            sphereCastInfo.castDistance = castDistance;
            sphereCastInfo.radius = radius;
            sphereCastInfo.ray = ray;

            float closestDistance = float.MaxValue;
            sphereCastInfo.closestDistanceRaycastHitIndex = -1;

            //float mostSimilarProduct = float.MaxValue;
            sphereCastInfo.closestNormalRaycastHitIndex = -1;

            sphereCastInfo.raycastHits = Physics.SphereCastAll(sphereCastInfo.ray.origin, sphereCastInfo.radius,
                sphereCastInfo.ray.direction, sphereCastInfo.castDistance, layerMask);
            
            for (int i = 0; i < sphereCastInfo.raycastHits.Length; i++)
            {
                if (sphereCastInfo.raycastHits[i].distance == 0f)
                    continue;

                Vector3 pointDirection = sphereCastInfo.raycastHits[i].point - basePosition;
                float angle = Vector3.Angle(ray.direction, pointDirection);
                if(angle > maxAngle)
                    continue;

                float distanceToSurface = Mathf.Abs(pointDirection.magnitude - radius);
                if(distanceToSurface < closestDistance)
                {
                    closestDistance = distanceToSurface;
                    sphereCastInfo.closestDistanceRaycastHitIndex = i;
                    sphereCastInfo.closestNormalRaycastHitIndex = i;
                }

                // TODO: is this still needed?
                /*
                float slopeProduct = Vector3.Dot(sphereCastInfo.raycastHits[i].normal, ray.direction);
                if (
                    sphereCastInfo.raycastHits[i].distance < closestNormalMaxDistance &&
                    slopeProduct < mostSimilarProduct)
                {
                    mostSimilarProduct = slopeProduct;
                    sphereCastInfo.closestNormalRaycastHitIndex = i;
                }

                if (sphereCastInfo.raycastHits[i].distance < closestDistance)
                {
                    closestDistance = sphereCastInfo.raycastHits[i].distance;
                    sphereCastInfo.closestDistanceRaycastHitIndex = i;
                }
                */
            }
        }
    }
}