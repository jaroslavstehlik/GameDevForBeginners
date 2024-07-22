using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chc
{
    struct SphereCastInfo
    {
        public bool collides;
        public RaycastHit closestNormalRaycastHit;
        public RaycastHit closestDistanceRaycastHit;
        public RaycastHit[] raycastHits;
        public Ray ray;
        public float radius;
        public float castDistance;
    }

    struct CollisionState
    {
        public SphereCastInfo groundInfo;
    }

    struct PlayerInput
    {
        public Vector2 move;
        public bool jump;
        public bool crouch;
        public bool sprint;

        public static PlayerInput Empty
        {
            get { return new PlayerInput() { move = Vector2.zero, jump = false, sprint = false }; }
        }

        public bool isEmpty
        {
            get { return move == Vector2.zero && jump == false && sprint == false; }
        }
    }

    public class CharacterControllerRunner : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        public LayerMask environmentMask = int.MaxValue;
        public float moveSpeed = 5f;
        public float sprintMultiplier = 2f;
        public float crouchMultiplier = 0.5f;
        public bool visualiseDebug = true;
        public bool showGroundHit = true;

        private Queue<PlayerInput> _playerInputQueue = new Queue<PlayerInput>();
        private CollisionState _collisionState;

        private void Update()
        {
            PlayerInput playerInput = PlayerInput.Empty;

            if (Input.GetKey(KeyCode.W))
            {
                playerInput.move.y = 1f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                playerInput.move.y = -1f;
            }
            else
            {
                playerInput.move.y = 0f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                playerInput.move.x = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                playerInput.move.x = 1f;
            }
            else
            {
                playerInput.move.x = 0f;
            }

            playerInput.jump = Input.GetKeyDown(KeyCode.Space);
            playerInput.sprint = Input.GetKey(KeyCode.LeftShift);
            playerInput.crouch = Input.GetKey(KeyCode.LeftControl);

            if (!playerInput.isEmpty)
            {
                _playerInputQueue.Enqueue(playerInput);
            }
        }

        void FixedUpdate()
        {
            Vector3 position = _rigidbody.position;
            Quaternion rotation = _rigidbody.rotation;
            Matrix4x4 localToWorld = Matrix4x4.TRS(position, rotation, Vector3.one);
            Matrix4x4 worldToLocal = localToWorld.inverse;

            float closestNormalMaxDistance = 0.25f;
            float groundOffset = 0.1f;
            float rayCastDistance = 10.0f;
            Vector3 up = rotation * Vector3.up;
            Ray groundRay = new Ray(_rigidbody.position + up * _collider.radius + up * groundOffset, -up);
            SphereCast(out _collisionState.groundInfo, groundRay, rayCastDistance, _collider.radius, environmentMask,
                closestNormalMaxDistance);

            // consume the queue and obtain latest playerInput
            PlayerInput playerInput = new PlayerInput();
            while (_playerInputQueue.Count > 0)
            {
                PlayerInput newPlayerInput = _playerInputQueue.Dequeue();
                playerInput.jump |= newPlayerInput.jump;
                playerInput.sprint |= newPlayerInput.sprint;
                playerInput.crouch |= newPlayerInput.crouch;
                playerInput.move = newPlayerInput.move;
            }

            float playerSpeed = moveSpeed;
            if (playerInput.crouch)
            {
                playerSpeed *= crouchMultiplier;
            }
            else if (playerInput.sprint)
            {
                playerSpeed *= sprintMultiplier;
            }

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;
            
            Vector3 localGroundNormal =
                worldToLocal.MultiplyVector(_collisionState.groundInfo.closestNormalRaycastHit.normal);

            float rampDistance = Mathf.Max(0f,
                _collisionState.groundInfo.closestDistanceRaycastHit.distance - groundOffset);
            float rampLocalAngle = Vector3.Angle(localGroundNormal, Vector3.up);

            // project movement based on plane normal
            // This makes walking on edges and thin walls much more predictable
            Vector3 playerMove = ProjectVelocityOnNormal(playerInputDirection, playerInputMagnitude, up, localGroundNormal);
            
            Vector3 futureVelocity = rotation * playerMove;

            Vector3 gravityDirection = Physics.gravity.normalized;
            futureVelocity += gravityDirection * rampDistance / Time.fixedDeltaTime;

            float cameraYaw = _cameraTransform.rotation.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
            _rigidbody.velocity = futureVelocity;
        }

        static Vector3 ProjectVelocityOnNormal(Vector3 velocityDirection, float velocityMagnitude, Vector3 playerUp, Vector3 groundNormal)
        {
            Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(velocityDirection, playerUp));
            Vector3 tangent = Vector3.Normalize(Vector3.Cross(groundNormal, rotationAxis));
            return tangent * velocityMagnitude;
        }

        static void SphereCast(out SphereCastInfo sphereCastInfo, Ray ray, float castDistance, float radius,
            int layerMask,
            float closestNormalMaxDistance)
        {
            sphereCastInfo.castDistance = castDistance;
            sphereCastInfo.radius = radius;
            sphereCastInfo.ray = ray;

            sphereCastInfo.raycastHits = Physics.SphereCastAll(sphereCastInfo.ray.origin, sphereCastInfo.radius,
                sphereCastInfo.ray.direction, sphereCastInfo.castDistance, layerMask);
            float mostSimilarProduct = float.MaxValue;
            int mostSimilarRaycastHitIndex = -1;

            float closestDistance = float.MaxValue;
            int closestDistanceRaycastHitIndex = -1;

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
                    mostSimilarRaycastHitIndex = i;
                }

                if (sphereCastInfo.raycastHits[i].distance < closestDistance)
                {
                    closestDistance = sphereCastInfo.raycastHits[i].distance;
                    closestDistanceRaycastHitIndex = i;
                }
            }

            sphereCastInfo.collides = mostSimilarRaycastHitIndex != -1 || closestDistanceRaycastHitIndex != -1;

            if (mostSimilarRaycastHitIndex != -1)
            {
                sphereCastInfo.closestNormalRaycastHit = sphereCastInfo.raycastHits[mostSimilarRaycastHitIndex];
            }
            else
            {
                sphereCastInfo.closestNormalRaycastHit = new RaycastHit();
            }

            if (closestDistanceRaycastHitIndex != -1)
            {
                sphereCastInfo.closestDistanceRaycastHit = sphereCastInfo.raycastHits[closestDistanceRaycastHitIndex];
            }
            else
            {
                sphereCastInfo.closestDistanceRaycastHit = new RaycastHit();
            }
        }

        void DrawSphereCast(SphereCastInfo sphereCastInfo)
        {
            Gizmos.color = sphereCastInfo.collides ? Color.red : Color.white;
            Gizmos.DrawWireSphere(sphereCastInfo.ray.origin, sphereCastInfo.radius);
            Gizmos.DrawWireSphere(
                sphereCastInfo.ray.origin + sphereCastInfo.ray.direction * sphereCastInfo.castDistance,
                sphereCastInfo.radius);
            if (sphereCastInfo.collides)
            {
                Gizmos.DrawWireSphere(sphereCastInfo.closestNormalRaycastHit.point, 0.1f);
            }
        }

        private void OnDrawGizmos()
        {
            if (!visualiseDebug)
                return;

            if (showGroundHit)
                DrawSphereCast(_collisionState.groundInfo);
            if (_collisionState.groundInfo.collides)
            {
                Gizmos.DrawWireSphere(_collisionState.groundInfo.closestNormalRaycastHit.point, 0.1f);
                Gizmos.DrawLine(_collisionState.groundInfo.closestNormalRaycastHit.point,
                    _collisionState.groundInfo.closestNormalRaycastHit.point +
                    _collisionState.groundInfo.closestNormalRaycastHit.normal);
            }
        }
    }
}