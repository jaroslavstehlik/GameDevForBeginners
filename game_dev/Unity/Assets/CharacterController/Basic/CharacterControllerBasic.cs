using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct GroundInfo
{
    public bool grounded;
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
    public GroundInfo groundInfo;
    public ProximityInfo[] sideProximity;
}
    
struct PlayerInput
{
    public Vector2 move;
    public bool jump;

    public static PlayerInput Empty
    {
        get{
            return new PlayerInput() { move = Vector2.zero, jump = false };
        }
    }

    public bool isEmpty
    {
        get { return move == Vector2.zero && jump == false; }
    }
}

public class CharacterControllerBasic : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _groundTrigger;
    [SerializeField] private Transform _leftWallTrigger;
    [SerializeField] private Transform _rightWallTrigger;
    [SerializeField] private Transform _forwardWallTrigger;
    [SerializeField] private Transform _backwardWallTrigger;
    public LayerMask environmentMask = int.MaxValue;
    public float raycastLength = 2f;

    
    public float moveSpeed = 1f;
    public float jumpAmount = 10f;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerInput.jump = true;
        }
        else
        {
            playerInput.jump = false;
        }
        
        if (!playerInput.isEmpty)
        {
            _playerInputQueue.Enqueue(playerInput);
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = _rigidbody.velocity;
        Quaternion rotation = _rigidbody.rotation;
        // modify rb velocity to prevent collider wall clipping
        
        // pre simulate fixed update
        // PreSimulationUpdate
        
        // capture rigidbody velocity
        
        // ProcessVelocity
        
        // Grounded
        
        // Ungrounded

        // user movement

        DetectGround(out _collisionState.groundInfo);
        
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
            playerInput = _playerInputQueue.Dequeue();
        }

        Vector3 playerMove = new Vector3(playerInput.move.x, 0f, playerInput.move.y);
        Vector3 futureMove = playerMove;
        Vector3 futureVelocity = Vector3.zero;

        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;

        for (int i = 0; i < _collisionState.sideProximity.Length; i++)
        {
            if(!_collisionState.sideProximity[i].hit)
                continue;

            Vector3 normal = _collisionState.sideProximity[i].raycastHit.normal;
            Vector3 localNormal = worldToLocal.MultiplyVector(normal);
            Vector2 localNormal2D = new Vector2(localNormal.x, localNormal.z);

            /*
            float dot = Vector2.Dot(_collisionState.sideProximity[i].localDirection, localNormal2D) * -1f;
            float dot2 = Vector2.Dot(_collisionState.sideProximity[i].localDirection, playerMove);
            float dot3 = dot * dot2;
            
            if (dot3 > 0f)
            {
                //float dot2 = Vector2.Dot(_collisionState.sideProximity[i].localDirection, playerInput.move);
                Vector3 oposingDirection = new Vector3(_collisionState.sideProximity[i].localDirection.x,
                    0f,
                    _collisionState.sideProximity[i].localDirection.y);

                float raycastDistanceInverse = Mathf.Clamp01(_collisionState.sideProximity[i].raycastLength - _collisionState.sideProximity[i].raycastHit.distance) / _collisionState.sideProximity[i].raycastLength;
                futureMove -= oposingDirection * dot * Mathf.Pow(raycastDistanceInverse, 2f);
            }
            */
        }
        
        futureVelocity = rotation * futureMove * moveSpeed;

        if (!_collisionState.groundInfo.grounded)
        {
            futureVelocity += Physics.gravity * Time.fixedDeltaTime;
        }
        
        if (_collisionState.groundInfo.grounded && playerInput.jump)
        {
            futureVelocity += Vector3.up * jumpAmount;
        }

        velocity += futureVelocity;
        _rigidbody.velocity = velocity;
    }

    void DetectGround(out GroundInfo groundInfo)
    {
        groundInfo.castDistance = 0.25f;
        groundInfo.radius = _collider.radius;
        Vector3 up = transform.up;
        Vector3 down = -up;
        Vector3 origin = _rigidbody.position + up * groundInfo.radius + up * groundInfo.castDistance * 0.5f;

        groundInfo.ray = new Ray(origin, down);
        groundInfo.grounded =
            Physics.SphereCast(groundInfo.ray.origin, groundInfo.radius, groundInfo.ray.direction, out groundInfo.raycastHit, groundInfo.castDistance, environmentMask);
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
    
    private void OnDrawGizmos()
    {
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

        Gizmos.color = _collisionState.groundInfo.grounded ? Color.red : Color.white;
        Gizmos.DrawWireSphere(_collisionState.groundInfo.ray.origin, _collisionState.groundInfo.radius);
        Gizmos.DrawWireSphere(_collisionState.groundInfo.ray.origin + _collisionState.groundInfo.ray.direction * _collisionState.groundInfo.castDistance, _collisionState.groundInfo.radius);
        if (_collisionState.groundInfo.grounded)
        {
            Gizmos.DrawWireSphere(_collisionState.groundInfo.raycastHit.point, 0.1f);
        }
    }
}
