using UnityEngine;

/*
To detect player jump, we have to detect that player is standing on something.
We call this state grounded. When player is not standing on ground
we call this ungrounded or falling.

Raycasts: we can cast rays from players feet to find ground distance.
The problem with raycasts is that they are infinitely small, therefore our
ray can easily end up in a tiny crack and the player could think that its not standing on floor.

Circlecasts | Spherecasts: are basically thick raycasts. They prevent the problem with little cracks
in our floor.

BoxOverlay : we can overlap a box inside players feet to detect if it overlaps any colliders which
we think is the floor.

In all of the cases we have to differentiate between colliders from the player and the environment.
We can use layers for that.
*/

public enum PlayerState
{
    falling,
    grounded,
    jumped,
}

public class PlayerJumping : MonoBehaviour
{
    public PlayerInputController playerInputController;
    public Rigidbody2D rigidbody;
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;
    public float jumpHeight = 2f;
    public BoxCollider2D feetCollider;
    public LayerMask feetColliderLayerMask;
    
    private int _minJumpDurationPhysicsFrames = 3;
    private int _jumpDurationFixedFrames = 0;

    // We have to make sure that player is standing on something first.
    private PlayerState _playerState = PlayerState.falling;
    private Vector2 _lastJumpPosition = Vector2.zero;
    private Vector2 _capturedVelocity = Vector2.zero;
    private Vector2 _capturedPosition = Vector2.zero;
    private Vector2 _lastCapturedPosition = Vector2.zero;

    bool DetectGround()
    {
        Vector3 feetColliderPosition = feetCollider.transform.TransformPoint(feetCollider.offset);
        float feetColliderRotation = feetCollider.transform.rotation.eulerAngles.z;
        Vector2 feetColliderSize = feetCollider.size;
        return Physics2D.OverlapBox(feetColliderPosition, feetColliderSize, feetColliderRotation,
            feetColliderLayerMask);
    }

    void StartFalling()
    {
        ApplyGravity();
        // switch to falling state
        _playerState = PlayerState.falling;
    }

    void StartJump()
    {
        ZeroGravity();
        // capture our last position during jump
        _lastJumpPosition = _capturedPosition;
        // switch to jump state
        _playerState = PlayerState.jumped;
        // reset jump duration
        _jumpDurationFixedFrames = 0;
    }

    void StartGrounded()
    {
        ApplyGravity();
        // switch to grounded state
        _playerState = PlayerState.grounded;
    }

    void ApplyGravity()
    {
        rigidbody.gravityScale = 1f;
    }

    void ZeroGravity()
    {
        rigidbody.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        // capture velocity
        _capturedVelocity = rigidbody.linearVelocity;
        // capture position 
        _lastCapturedPosition = _capturedPosition;
        _capturedPosition = rigidbody.position;
        
        bool grounded = DetectGround();
        switch (_playerState)
        {
            case PlayerState.falling:
                FallingState(grounded);
                break;
            case PlayerState.grounded:
                GroundedState(grounded);
                break;
            case PlayerState.jumped:
                JumpState();
                break;
        }

        _capturedVelocity.x = playerInputController.playerInput.move.x * moveSpeed;
        rigidbody.linearVelocity = _capturedVelocity;
    }

    private void FallingState(bool grounded)
    {
        // are we standing on floor?
        if (grounded)
        {
            StartGrounded();
            return;
        }
        
        ApplyGravity();
    }

    private void JumpState()
    {
        // our current vertical speed
        float verticalMagnitude = Mathf.Abs(_lastCapturedPosition.y - _capturedPosition.y);
        // our height compared to our jump start position
        float verticalHeight = _capturedPosition.y - _lastJumpPosition.y;
        // calculate percentage (0 to 1) representing jump start to jump finish
        float jumpProgress = 1f - Mathf.Clamp01(verticalHeight/jumpHeight);
        // change velocity based on jump progress
        float verticalVelocity = jumpSpeed * jumpProgress;
        // clamp velocity so it does not exceed jump height.
        float maxVelocity = jumpHeight / Time.fixedDeltaTime;
        _capturedVelocity.y = Mathf.Clamp(verticalVelocity, 0f, maxVelocity);
        
        // we have to wait several physics frames before we start detecting falling 
        if (_jumpDurationFixedFrames <= _minJumpDurationPhysicsFrames)
        {
            // Increase physics frame by 1
            _jumpDurationFixedFrames += 1;
            ZeroGravity();
            return;
        }
        
        // detect if we got stuck, probably by hitting the ceiling
        bool gotStuck = verticalMagnitude < 0.01f;
        // detect if we reached jump height
        bool reachedJumpHeight = verticalHeight >= jumpHeight;
        
        // start falling when we stop moving or reach jump height
        if  (gotStuck || reachedJumpHeight)
        {
            StartFalling();
            return;
        }

        ZeroGravity();
    }

    private void GroundedState(bool grounded)
    {
        if (!grounded)
        {
            StartFalling();
            return;
        }
        
        // detect jump
        if (playerInputController.playerInput.jump.isPressed)
        {
            StartJump();
            return;
        }

        // when grounded do not apply gravity to prevent sliding from slopes.
        ZeroGravity();
    }
}