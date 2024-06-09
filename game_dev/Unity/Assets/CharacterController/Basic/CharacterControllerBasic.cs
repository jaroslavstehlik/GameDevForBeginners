using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct SphereCastInfo
{
    public bool collides;
    public RaycastHit raycastHit;
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
    public float jumpAmount = 10f;
    public float jumpDuration = 1f;
    public bool visualiseDebug = true;
    
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerInput.jump = true;
        }
        else
        {
            playerInput.jump = false;
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerInput.sprint = true;
        }
        else
        {
            playerInput.sprint = false;
        }
        
        if (!playerInput.isEmpty)
        {
            _playerInputQueue.Enqueue(playerInput);
        }
    }

    void FixedUpdate()
    {
        Quaternion rotation = _rigidbody.rotation;

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
            playerInput.move = newPlayerInput.move;
        }

        float playerSpeed = moveSpeed;
        if (playerInput.sprint)
            playerSpeed *= sprintMultiplier;
        
        Vector2 playerInputDirection = playerInput.move.normalized;
        float playerInputMagnitude = Mathf.Clamp(playerInput.move.magnitude, 0f, 1f) * playerSpeed;
        Vector2 playerInputMove = playerInputDirection * playerInputMagnitude;
        Vector3 playerMove = new Vector3(playerInputMove.x, 0f, playerInputMove.y);
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
            }
        }
        else
        {
            if (!_collisionState.groundInfo.collides)
            {
                futureVelocity += Physics.gravity;
            }
            else
            {
                if (playerInput.jump)
                {
                    jumpTimeRemaining = jumpDuration;
                    futureVelocity += Vector3.up * jumpAmount;
                }
            }

        }
        
        _rigidbody.velocity = futureVelocity;
    }

    static void SphereCast(out SphereCastInfo sphereCastInfo, Ray ray, float castDistance, float radius, int layerMask)
    {
        sphereCastInfo.castDistance = castDistance;
        sphereCastInfo.radius = radius;
        sphereCastInfo.ray = ray;
        sphereCastInfo.collides =
            Physics.SphereCast(sphereCastInfo.ray.origin, sphereCastInfo.radius, sphereCastInfo.ray.direction, out sphereCastInfo.raycastHit, sphereCastInfo.castDistance, layerMask);
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

        DrawSphereCast(_collisionState.groundInfo);
        DrawSphereCast(_collisionState.ceilingInfo);
    }
}
