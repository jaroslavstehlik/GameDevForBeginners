using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct SphereCastInfo
{
    public bool collides;
    public RaycastHit raycastHit;
    public RaycastHit[] raycastHits;
    public Ray ray;
    public float radius;
    public float castDistance;
}

struct ProximityInfo
{
    public bool hit;
    public Ray raycast;
    public float raycastLength;
    public Vector2 localDirection;
    public RaycastHit raycastHit;
}

struct CollisionState
{
    public SphereCastInfo ceilingInfo;
    public SphereCastInfo groundInfo;
    public ProximityInfo[] sideProximity;
}
    
struct PlayerInput
{
    public Vector2 move;
    public bool jump;
    public bool crouch;
    public bool sprint;

    public static PlayerInput Empty
    {
        get{
            return new PlayerInput() { move = Vector2.zero, jump = false, sprint = false };
        }
    }

    public bool isEmpty
    {
        get { return move == Vector2.zero && jump == false && sprint == false; }
    }
}

public class CharacterControllerBasic : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    public LayerMask environmentMask = int.MaxValue;
    public float raycastLength = 2f;
    public float moveSpeed = 1f;
    public float sprintMultiplier = 2f;
    public float crouchMultiplier = 0.5f;
    public float jumpAmount = 10f;
    public float jumpDuration = 1f;
    public float maxSlopeAngle = 45f;
    public bool visualiseDebug = true;
    public bool showGroundHit = true;
    public bool showCeilingHit = true;
    
    private Queue<PlayerInput> _playerInputQueue = new Queue<PlayerInput>();
    private CollisionState _collisionState;
    private float jumpTimeRemaining = 0f;
    
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

        float rayCastDistance = 0.25f;
        Vector3 up = rotation * Vector3.up;
        Ray groundRay = new Ray(_rigidbody.position + up * _collider.radius + up * rayCastDistance * 0.5f,
            -up);
        SphereCast(out _collisionState.groundInfo, groundRay, rayCastDistance, _collider.radius, environmentMask);

        Ray ceilingRay = new Ray(_rigidbody.position + up * _collider.height - up * _collider.radius - up * rayCastDistance * 0.5f,
            up);
        SphereCast(out _collisionState.ceilingInfo, ceilingRay, rayCastDistance, _collider.radius, environmentMask);

        Array.Resize(ref _collisionState.sideProximity, 8);
        for (int i = 0; i < _collisionState.sideProximity.Length; i++)
        {
            float angleProgress = i / (float)(_collisionState.sideProximity.Length);
            float angle = angleProgress * 360f;

            float baseOffset = 0.25f;
            Vector3 origin = transform.position + transform.up * baseOffset;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * Vector3.forward;
            Vector3 localDirection = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
            
            float capsueSize = _collider.radius * 0.5f;

            _collisionState.sideProximity[i] = new ProximityInfo();
            _collisionState.sideProximity[i].raycastLength = raycastLength;
            _collisionState.sideProximity[i].localDirection = new Vector2(
                localDirection.x,
                localDirection.z
                );
            
            _collisionState.sideProximity[i].raycast = new Ray(origin + direction * capsueSize, direction);
            _collisionState.sideProximity[i].hit = Physics.Raycast(
                _collisionState.sideProximity[i].raycast,
                out _collisionState.sideProximity[i].raycastHit,
                _collisionState.sideProximity[i].raycastLength,
                environmentMask);
        }
        
        // consume the queue and obtain latest playerInput
        PlayerInput playerInput = new PlayerInput();
        while(_playerInputQueue.Count > 0)
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
        else if(playerInput.sprint)
        {
            playerSpeed *= sprintMultiplier;    
        }

        Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
        float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;
        Vector3 playerInputMove = playerInputDirection * playerInputMagnitude;
        
        Vector3 localGroundNormal = worldToLocal.MultiplyVector(_collisionState.groundInfo.raycastHit.normal);

        float rampLocalAngle = Vector3.Angle(localGroundNormal, Vector3.up);
        bool slopeIsTooSteep = !_collisionState.groundInfo.collides || rampLocalAngle > maxSlopeAngle;
        
        if (slopeIsTooSteep)
        {
            float rampPLayerLocalDotProduct = Vector3.Dot(playerInputDirection, localGroundNormal);
            if (rampPLayerLocalDotProduct < 0f)
            {
                playerInputMove *= 0f;
            }
        }

        Vector3 playerMove = playerInputMove;
        Vector3 futureVelocity = rotation * playerMove;
        
        if (jumpTimeRemaining > 0f)
        {
            if (!_collisionState.ceilingInfo.collides)
            {
                jumpTimeRemaining -= Time.fixedDeltaTime;
                futureVelocity += Vector3.up * jumpAmount;
            }
            else
            {
                jumpTimeRemaining = 0f;
                ApplyGravity(ref futureVelocity);
            }
        }
        else
        {
            if (!_collisionState.groundInfo.collides)
            {
                ApplyGravity(ref futureVelocity);
            }
            else
            {
                if (slopeIsTooSteep)
                {
                    ApplyGravity(ref futureVelocity);
                }
                else if(playerInput.jump)
                {
                    jumpTimeRemaining = jumpDuration;
                    futureVelocity += Vector3.up * jumpAmount;
                }
            }

        }
        
        _rigidbody.velocity = futureVelocity;
    }

    void ApplyGravity(ref Vector3 velocity)
    {
        velocity += Physics.gravity;
    }

    static void SphereCast(out SphereCastInfo sphereCastInfo, Ray ray, float castDistance, float radius, int layerMask)
    {
        sphereCastInfo.castDistance = castDistance;
        sphereCastInfo.radius = radius;
        sphereCastInfo.ray = ray;
        
        sphereCastInfo.raycastHits = Physics.SphereCastAll(sphereCastInfo.ray.origin, sphereCastInfo.radius, sphereCastInfo.ray.direction, sphereCastInfo.castDistance, layerMask);
        float mostSimilarProduct = float.MaxValue;
        int mostSimilarRaycastHitIndex = -1;
        for (int i = 0; i < sphereCastInfo.raycastHits.Length; i++)
        {
            if(sphereCastInfo.raycastHits[i].distance == 0f)
                continue;
            
            float slopeProduct = Vector3.Dot(sphereCastInfo.raycastHits[i].normal, ray.direction);
            if (slopeProduct < mostSimilarProduct)
            {
                mostSimilarProduct = slopeProduct;
                mostSimilarRaycastHitIndex = i;
            }
        }
        
        sphereCastInfo.collides = mostSimilarRaycastHitIndex != -1;
        sphereCastInfo.raycastHit = sphereCastInfo.collides ? sphereCastInfo.raycastHits[mostSimilarRaycastHitIndex] : new RaycastHit();
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
        Gizmos.DrawWireSphere(sphereCastInfo.ray.origin + sphereCastInfo.ray.direction * sphereCastInfo.castDistance, sphereCastInfo.radius);
        if (sphereCastInfo.collides)
        {
            Gizmos.DrawWireSphere(sphereCastInfo.raycastHit.point, 0.1f);
        }
    }
    
    private void OnDrawGizmos()
    {
        if(!visualiseDebug)
            return;
        
        /*
        if (_collisionState.sideProximity != null)
        {
            for (int i = 0; i < _collisionState.sideProximity.Length; i++)
            {
                Gizmos.color = Color.white;
                if (_collisionState.sideProximity[i].hit)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(_collisionState.sideProximity[i].raycastHit.point, 
                        _collisionState.sideProximity[i].raycastHit.point + _collisionState.sideProximity[i].raycastHit.normal);
                }
                
                Gizmos.DrawLine(_collisionState.sideProximity[i].raycast.origin,
                    _collisionState.sideProximity[i].raycast.origin + _collisionState.sideProximity[i].raycast.direction * _collisionState.sideProximity[i].raycastLength);
            }
        }
        */
        if(showGroundHit)
            DrawSphereCast(_collisionState.groundInfo);
        if(showCeilingHit)
            DrawSphereCast(_collisionState.ceilingInfo);
        if (_collisionState.groundInfo.collides)
        {
            Gizmos.DrawWireSphere(_collisionState.groundInfo.raycastHit.point, 0.1f);
            Gizmos.DrawLine(_collisionState.groundInfo.raycastHit.point, _collisionState.groundInfo.raycastHit.point + _collisionState.groundInfo.raycastHit.normal);
            
            for (int i = 0; i < _collisionState.groundInfo.raycastHits.Length; i++)
            {
                //Gizmos.DrawLine(_collisionState.groundInfo.raycastHits[i].point, _collisionState.groundInfo.raycastHits[i].point + _collisionState.groundInfo.raycastHits[i].normal);       
            }
        }
    }
}
