using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
    public SphereCastInfo ceilingInfo;
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

public class CharacterControllerBasic : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    public LayerMask environmentMask = int.MaxValue;
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float crouchMultiplier = 0.5f;
    public float jumpAmount = 10f;
    public float jumpDuration = 1f;
    public float fallDuration = 1f;
    public float maxSlopeAngle = 45f;
    public bool visualiseDebug = true;
    public bool showGroundHit = true;
    public bool showCeilingHit = true;

    private Queue<PlayerInput> _playerInputQueue = new Queue<PlayerInput>();
    private CollisionState _collisionState;
    private float jumpTimeRemaining = 0f;

    public float jumpProgress
    {
        get { return 1f - Mathf.Clamp01(jumpTimeRemaining / jumpDuration); }
    }

    private float fallTimeRemaining = 0f;

    public float fallProgress
    {
        get { return Mathf.Clamp01(fallTimeRemaining / fallDuration); }
    }

    private Vector3 gravityVelocity = Vector3.zero;

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

        Ray ceilingRay = new Ray(
            _rigidbody.position + up * _collider.height - up * _collider.radius - up * _collider.height * 0.5f,
            up);
        SphereCast(out _collisionState.ceilingInfo, ceilingRay, rayCastDistance, _collider.radius, environmentMask,
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

        Vector3 localGroundNormal = worldToLocal.MultiplyVector(_collisionState.groundInfo.closestNormalRaycastHit.normal);
        
        float rampDistance =
            Mathf.Max(0f, _collisionState.groundInfo.closestDistanceRaycastHit.distance - groundOffset);
        float rampLocalAngle = Vector3.Angle(localGroundNormal, Vector3.up);
        bool isTooSteep = rampLocalAngle > maxSlopeAngle;
        bool isGrounded = _collisionState.groundInfo.collides && rampDistance < 0.25f;
        //Debug.Log($"rampDistance: {rampDistance}, rampLocalAngle: {rampLocalAngle}");

        // project movement based on plane normal
        // This makes walking on edges and thin walls much more predictable
        Vector3 playerMove = playerInputDirection * playerInputMagnitude;
        if (isGrounded)
        {
            playerMove = ProjectVelocityOnNormal(playerInputDirection, playerInputMagnitude, up, localGroundNormal);
        }
        
        if (isTooSteep)
        {
            float rampPLayerLocalDotProduct = Vector3.Dot(playerInputDirection, localGroundNormal);
            if (rampPLayerLocalDotProduct < 0f)
            {
                playerMove *= 0f;
            }
        }
        
        Vector3 futureVelocity = rotation * playerMove;
        bool applyGravity = false;
        bool applyJump = false;

        // jumping
        if (jumpTimeRemaining > 0f)
        {
            // Hit celining
            if (!_collisionState.ceilingInfo.collides)
            {
                jumpTimeRemaining = Mathf.Clamp(jumpTimeRemaining - Time.fixedDeltaTime, 0, jumpDuration);
                fallTimeRemaining = 0f;
                applyJump = true;
            }
            else
            {
                // Stopped jumping
                jumpTimeRemaining = 0f;
                fallTimeRemaining = 0f;
                applyGravity = true;
            }
        }
        else
        {
            // falling
            if (!isGrounded)
            {
                applyGravity = true;
                fallTimeRemaining = Mathf.Clamp(fallTimeRemaining + Time.fixedDeltaTime, 0f, fallDuration);
            }
            else
            {
                // falling
                if (isTooSteep)
                {
                    applyGravity = true;
                    fallTimeRemaining = Mathf.Clamp(fallTimeRemaining + Time.fixedDeltaTime, 0f, fallDuration);
                }

                // about to jump
                else if (playerInput.jump)
                {
                    jumpTimeRemaining = jumpDuration;
                    fallTimeRemaining = 0f;
                    ApplyJump(ref futureVelocity, jumpProgress);
                }
                // standing
                else
                {
                    // Pin to ground
                    Vector3 gravityDirection = Physics.gravity.normalized;
                    futureVelocity += gravityDirection * rampDistance / Time.fixedDeltaTime;

                    fallTimeRemaining = 0f;
                }
            }
        }

        if (applyGravity)
        {
            ApplyGravity(ref futureVelocity, fallProgress);
        }
        else
        {
            ResetGravity(ref gravityVelocity);
        }

        if (applyJump)
        {
            ApplyJump(ref futureVelocity, jumpProgress);
        }

        float cameraYaw = _cameraTransform.rotation.eulerAngles.y;
        _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
        _rigidbody.velocity = futureVelocity;
    }

    void ResetGravity(ref Vector3 gravityVelocity)
    {
        Debug.Log("ResetGravity");
        gravityVelocity = Vector3.zero;
    }

    void ApplyGravity(ref Vector3 velocity, float fallProgress)
    {
        velocity += Physics.gravity * fallProgress;
        Debug.Log("ApplyGravity");
    }

    void ApplyJump(ref Vector3 velocity, float jumpProgress)
    {
        velocity += Vector3.up * jumpAmount * (1f - jumpProgress);
        Debug.Log("ApplyJump");
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

    void DrawTriggerGizmo(Transform transform, bool triggered)
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        if (triggered)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
        }

        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }

    void DrawSphereCast(SphereCastInfo sphereCastInfo)
    {
        Gizmos.color = sphereCastInfo.collides ? Color.red : Color.white;
        Gizmos.DrawWireSphere(sphereCastInfo.ray.origin, sphereCastInfo.radius);
        Gizmos.DrawWireSphere(sphereCastInfo.ray.origin + sphereCastInfo.ray.direction * sphereCastInfo.castDistance,
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
        if (showCeilingHit)
            DrawSphereCast(_collisionState.ceilingInfo);
        if (_collisionState.groundInfo.collides)
        {
            Gizmos.DrawWireSphere(_collisionState.groundInfo.closestNormalRaycastHit.point, 0.1f);
            Gizmos.DrawLine(_collisionState.groundInfo.closestNormalRaycastHit.point,
                _collisionState.groundInfo.closestNormalRaycastHit.point +
                _collisionState.groundInfo.closestNormalRaycastHit.normal);
        }
    }
}