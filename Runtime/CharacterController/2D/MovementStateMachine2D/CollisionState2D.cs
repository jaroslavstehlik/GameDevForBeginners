using System;
using UnityEngine;

namespace GameDevForBeginners
{
    public struct RectCastInfo
    {
        public RaycastHit2D[] raycastHits;
        public Ray2D ray;
        public Vector2 size;
        public float angle;
        public float castDistance;        

        internal int closestNormalRaycastHitIndex;
        internal int closestDistanceRaycastHitIndex;

        public Rigidbody2D GetRigidbody()
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

        public bool closestNormalRaycastHit(out RaycastHit2D raycastHit)
        {
            if (raycastHits == null ||
                closestNormalRaycastHitIndex < 0 ||
                closestNormalRaycastHitIndex >= raycastHits.Length)
            {
                raycastHit = new RaycastHit2D();
                return false;
            }

            raycastHit = raycastHits[closestNormalRaycastHitIndex]; 
            return true;
        }
        
        public bool closestDistanceRaycastHit(out RaycastHit2D raycastHit){
            if (raycastHits == null ||
                closestDistanceRaycastHitIndex < 0 ||
                closestDistanceRaycastHitIndex >= raycastHits.Length)
            {
                raycastHit = new RaycastHit2D();
                return false;
            }

            raycastHit = raycastHits[closestDistanceRaycastHitIndex];
            return true;
        }
        
        public void DebugDrawHits()
        {
#if UNITY_EDITOR            
            if(raycastHits == null)
                return;
            Gizmos.color = Color.red;
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
            Gizmos.color = Color.red;
            for (int i = 0; i < raycastHits.Length; i++)
            {
                Gizmos.DrawLine(raycastHits[i].point, raycastHits[i].point + raycastHits[i].normal * length);
            }
#endif
        }
    }

    public class CollisionInfo2D
    {
        public Vector2 normal { get; private set; }
        public float rampDistance { get; private set; }
        public float rampLocalAngle { get; private set; }
        public CollisionInfo2D(Vector2 up, Vector2 basePosition, Vector2 baseUp, RaycastHit2D closestDistanceRaycastHit, RaycastHit2D closestNormalRaycastHit)
        {
            normal = closestNormalRaycastHit.normal;
            Vector2 localPosition = closestDistanceRaycastHit.point - basePosition;
            rampDistance = Vector2.Dot(baseUp, localPosition);
            rampLocalAngle = Vector2.Angle(normal, up);
        }
    }
    
    public struct CollisionStateInfo2D
    {
        private Vector2 basePosition;
        private float baseRotation;
        public float maxSlopeAngle { get; private set; }
        public CollisionInfo2D collisionInfo { get; private set; }
        public RectCastInfo sphereCastInfo;
        public Matrix4x4 localToWorld => Matrix4x4.TRS(basePosition, Quaternion.Euler(0f, 0f, baseRotation), Vector3.one);
        public Matrix4x4 worldToLocal => localToWorld.inverse;        
        public Vector2 up => -Physics2D.gravity.normalized;
        public Vector2 localUp => worldToLocal.MultiplyVector(up);

        public CollisionStateInfo2D(RectCastDescriptor groundDetector, float maxSlopeAngle, LayerMask environmentMask)
        {
            this.collisionInfo = null;
            this.maxSlopeAngle = maxSlopeAngle;
            this.basePosition = groundDetector.basePosition;
            this.baseRotation = groundDetector.baseRotation;

            Ray2D groundRay = new Ray2D(groundDetector.rayOrigin, groundDetector.rayDirection);
            CollisionState2D.RectCast(out sphereCastInfo, groundRay, groundDetector.basePosition, groundDetector.size, groundDetector.angle, groundDetector.distance, environmentMask);

            groundDetector.isColliding = sphereCastInfo.collides;
            
            bool touching = true;
            touching &= sphereCastInfo.closestDistanceRaycastHit(out RaycastHit2D closestDistanceRaycastHit);
            touching &= sphereCastInfo.closestNormalRaycastHit(out RaycastHit2D closestNormalRaycastHit);
            if (touching)
            {
                float baseRotationRad = baseRotation * Mathf.Deg2Rad;
                collisionInfo = new CollisionInfo2D(up, basePosition, groundDetector.rayDirection, closestDistanceRaycastHit, closestNormalRaycastHit);
            }
        }
    }

    public class CollisionState2D : MonoBehaviour
    {
        [SerializeField] private RectCastDescriptor _groundDetector;
        [SerializeField] private RectCastDescriptor _ceilingDetector;
        public LayerMask environmentMask = int.MaxValue;
        
        public CollisionStateInfo2D GetGroundStateInfo(float maxSlopeAngle)
        {
            return new CollisionStateInfo2D(_groundDetector, maxSlopeAngle, environmentMask);
        }
        
        public CollisionStateInfo2D GetCeilingStateInfo()
        {
            return new CollisionStateInfo2D(_ceilingDetector, float.MaxValue, environmentMask);
        }

        public static void RectCast(out RectCastInfo rectCastInfo, Ray2D ray, Vector2 basePosition, Vector2 size, float angle, float castDistance, int layerMask)
        {
            rectCastInfo.castDistance = castDistance;
            rectCastInfo.size = size;
            rectCastInfo.ray = ray;
            rectCastInfo.angle = angle;

            float closestDistance = float.MaxValue;
            rectCastInfo.closestDistanceRaycastHitIndex = -1;
            rectCastInfo.closestNormalRaycastHitIndex = -1;

            rectCastInfo.raycastHits = Physics2D.BoxCastAll(ray.origin, rectCastInfo.size, angle, rectCastInfo.ray.direction, rectCastInfo.castDistance, layerMask);
            
            // find closest point to player base
            for (int i = 0; i < rectCastInfo.raycastHits.Length; i++)
            {
                if (rectCastInfo.raycastHits[i].distance == 0f)
                    continue;

                Vector2 localPosition = rectCastInfo.raycastHits[i].point - basePosition;
                float distanceY = Vector2.Dot(ray.direction, localPosition);

                if(distanceY < closestDistance)
                {
                    closestDistance = distanceY;
                    rectCastInfo.closestDistanceRaycastHitIndex = i;
                    rectCastInfo.closestNormalRaycastHitIndex = i;
                }                
            }            
        }
    }
}