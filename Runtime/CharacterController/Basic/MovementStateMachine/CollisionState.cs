using Chc;
using UnityEngine;

namespace GameDevForBeginners
{
    public struct SphereCastInfo
    {
        public RaycastHit[] raycastHits;
        internal int closestNormalRaycastHitIndex;
        internal int closestDistanceRaycastHitIndex;

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
        public Vector3 localGroundNormal { get; private set; }
        public float rampDistance { get; private set; }
        public float rampLocalAngle { get; private set; }
        public bool isTooSteep { get; private set; }
        public bool isGrounded { get; private set; }

        public GroundCollisionInfo(CollisionState collisionState, RaycastHit closestDistanceRaycastHit, RaycastHit closestNormalRaycastHit)
        {
            localGroundNormal = collisionState.worldToLocal.MultiplyVector(closestNormalRaycastHit.normal);
            rampDistance = Mathf.Max(0f, closestDistanceRaycastHit.distance - CollisionState.groundOffset);
            rampLocalAngle = Vector3.Angle(localGroundNormal, collisionState.up);
            isTooSteep = rampLocalAngle > collisionState.maxSlopeAngle;
            isGrounded = rampDistance < CollisionState.groundedDistance;
        }
    }
    
    public class CollisionState
    {
        private Vector3 position;
        private Quaternion rotation;
        public SphereCastInfo groundSphereCastInfo;
        public SphereCastInfo ceilingSphereCastInfo;
        public float maxSlopeAngle { get; private set; }
        public const float groundedDistance = 0.25f;
        public const float groundOffset = 0.15f;
        public GroundCollisionInfo groundCollisionInfo { get; private set; }
        public Vector3 up => rotation * Vector3.up;
        public Matrix4x4 localToWorld => Matrix4x4.TRS(position, rotation, Vector3.one);
        public Matrix4x4 worldToLocal => localToWorld.inverse;

        public void Update(Rigidbody rigidbody, Transform transform, SphereCast groundDetector, SphereCast ceilingDetector, LayerMask environmentMask,
            float maxSlopeAngle)
        {
            this.groundCollisionInfo = null;
            this.maxSlopeAngle = maxSlopeAngle;
            this.position = rigidbody.position;
            this.rotation = rigidbody.rotation;

            Vector3 localGroundDetectorPosition = transform.InverseTransformPoint(groundDetector.transform.position);
            
            Vector3 localCeilingDetectorPosition = transform.InverseTransformPoint(ceilingDetector.transform.position);
            Vector3 worldGroundDetectorPosition = rigidbody.position + rigidbody.rotation * localGroundDetectorPosition;
            Vector3 worldCeilingDetectorPosition = rigidbody.position + rigidbody.rotation * localCeilingDetectorPosition;
            
            Ray groundRay = new Ray(worldGroundDetectorPosition, groundDetector.transform.up);
            Debug.DrawRay(groundRay.origin, groundRay.direction * groundDetector.height);
            SphereCast(out groundSphereCastInfo, groundRay, groundDetector.height, groundDetector.radius,
                environmentMask,
                groundDetector.height);

            groundDetector.isColliding = groundSphereCastInfo.collides;
            
            Ray ceilingRay = new Ray(worldCeilingDetectorPosition, ceilingDetector.transform.up);
            Debug.DrawRay(ceilingRay.origin, ceilingRay.direction * ceilingDetector.height);
            SphereCast(out ceilingSphereCastInfo, ceilingRay, ceilingDetector.height, ceilingDetector.radius,
                environmentMask,
                ceilingDetector.height);
            
            ceilingDetector.isColliding = ceilingSphereCastInfo.collides;

            bool grounded = true;
            grounded &= groundSphereCastInfo.closestDistanceRaycastHit(out RaycastHit closestDistqnceRaycastHit);
            grounded &= groundSphereCastInfo.closestNormalRaycastHit(out RaycastHit closestNormalRaycastHit);
            if (grounded)
            {
                groundCollisionInfo = new GroundCollisionInfo(this, closestDistqnceRaycastHit, closestNormalRaycastHit);
            }
        }

        static void SphereCast(out SphereCastInfo sphereCastInfo, Ray ray, float castDistance, float radius,
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