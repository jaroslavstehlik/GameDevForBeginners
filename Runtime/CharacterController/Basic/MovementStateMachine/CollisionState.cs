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

    public class GroundCollisionInfo
    {
        public Vector3 groundNormal { get; private set; }
        public Vector3 localGroundNormal { get; private set; }
        public float rampDistance { get; private set; }
        public float rampLocalAngle { get; private set; }
        public bool isTooSteep { get; private set; }
        public bool isGrounded { get; private set; }

        public GroundCollisionInfo(Matrix4x4 worldToLocal, Vector3 up, float maxSlopeAngle, RaycastHit closestDistanceRaycastHit, RaycastHit closestNormalRaycastHit)
        {
            groundNormal = closestNormalRaycastHit.normal;
            localGroundNormal = worldToLocal.MultiplyVector(groundNormal);            
            rampDistance = Mathf.Max(0f, closestDistanceRaycastHit.distance - CollisionState.groundOffset);
            rampLocalAngle = Vector3.Angle(groundNormal, up);
            isTooSteep = rampLocalAngle > maxSlopeAngle;
            isGrounded = rampDistance < CollisionState.groundedDistance;
        }
    }
    
    public struct GroundStateInfo
    {
        private Vector3 position;
        private Quaternion rotation;
        public float maxSlopeAngle { get; private set; }
        public GroundCollisionInfo groundCollisionInfo { get; private set; }
        public SphereCastInfo sphereCastInfo;
        public Matrix4x4 localToWorld => Matrix4x4.TRS(position, rotation, Vector3.one);
        public Matrix4x4 worldToLocal => localToWorld.inverse;
        public Vector3 up => -Physics.gravity.normalized;
        public Vector3 localUp => worldToLocal.MultiplyVector(up);

        public GroundStateInfo(Rigidbody rigidbody, SphereCastDescriptor groundDetector, float maxSlopeAngle, LayerMask environmentMask)
        {
            this.groundCollisionInfo = null;
            this.maxSlopeAngle = maxSlopeAngle;
            this.position = rigidbody.position;
            this.rotation = rigidbody.rotation;

            Ray groundRay = new Ray(groundDetector.transform.position, groundDetector.transform.up);
            CollisionState.SphereCast(out sphereCastInfo, groundRay, groundDetector.height, groundDetector.radius,
                environmentMask,
                groundDetector.height);

            groundDetector.isColliding = sphereCastInfo.collides;
            
            bool grounded = true;
            grounded &= sphereCastInfo.closestDistanceRaycastHit(out RaycastHit closestDistanceRaycastHit);
            grounded &= sphereCastInfo.closestNormalRaycastHit(out RaycastHit closestNormalRaycastHit);
            if (grounded)
            {
                groundCollisionInfo = new GroundCollisionInfo(worldToLocal, up, maxSlopeAngle, closestDistanceRaycastHit, closestNormalRaycastHit);
            }
        }
    }

    public struct CeilingStateInfo
    {
        public SphereCastInfo sphereCastInfo;
        
        public CeilingStateInfo(SphereCastDescriptor ceilingDetector, LayerMask environmentMask)
        {
            Ray ceilingRay = new Ray(ceilingDetector.transform.position, ceilingDetector.transform.up);
            CollisionState.SphereCast(out sphereCastInfo, ceilingRay, ceilingDetector.height, ceilingDetector.radius,
                environmentMask,
                ceilingDetector.height);
            
            ceilingDetector.isColliding = sphereCastInfo.collides;
        }
    }

    public class CollisionState : MonoBehaviour
    {
        public const float groundedDistance = 0.25f;
        public const float groundOffset = 0.15f;

        [SerializeField] private SphereCastDescriptor _groundDetector;
        [SerializeField] private SphereCastDescriptor _ceilingDetector;
        public LayerMask environmentMask = int.MaxValue;
        
        public GroundStateInfo GetGroundStateInfo(Rigidbody rigidbody, float maxSlopeAngle)
        {
            return new GroundStateInfo(rigidbody, _groundDetector, maxSlopeAngle, environmentMask);
        }
        
        public CeilingStateInfo GetCeilingStateInfo()
        {
            return new CeilingStateInfo(_ceilingDetector, environmentMask);
        }

        public static void SphereCast(out SphereCastInfo sphereCastInfo, Ray ray, float castDistance, float radius,
            int layerMask,
            float closestNormalMaxDistance)
        {
            sphereCastInfo.castDistance = castDistance;
            sphereCastInfo.radius = radius;
            sphereCastInfo.ray = ray;

            float closestDistance = float.MaxValue;
            sphereCastInfo.closestDistanceRaycastHitIndex = -1;

            float mostSimilarProduct = float.MaxValue;
            sphereCastInfo.closestNormalRaycastHitIndex = -1;

            sphereCastInfo.raycastHits = Physics.SphereCastAll(sphereCastInfo.ray.origin, sphereCastInfo.radius,
                sphereCastInfo.ray.direction, sphereCastInfo.castDistance, layerMask);
            
            for (int i = 0; i < sphereCastInfo.raycastHits.Length; i++)
            {
                if (sphereCastInfo.raycastHits[i].distance == 0f)
                    continue;

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
            }
        }
    }
}