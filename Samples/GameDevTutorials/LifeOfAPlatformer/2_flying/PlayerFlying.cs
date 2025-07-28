using UnityEngine;

/*
 Player flying is applying velocity also in the vertical direction, usually
 in the opposing direction of gravity. Our lift force has to be higher than gravity force in order
 that we can actually fly. It is much simpler to turn off gravity during the jump.
 
 The reason why we instead turn off gravity is to make the jump instant, therefore it feels
 more responsive, rather than physically applying force over time like a rocket would do.
 */

public class PlayerFlying : MonoBehaviour
{
    public PlayerInputController playerInputController;
    public Rigidbody2D rigidbody;
    public float moveSpeed = 10f;
    public float flyingSpeed = 5f;
    
    private PlayerInput _playerInput = new PlayerInput();
    
    private void OnEnable()
    {
        playerInputController.onPlayerInputChanged += OnPlayerInputChanged;
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rigidbody.linearVelocity;
        velocity.x = _playerInput.move.x * moveSpeed;
        if (_playerInput.jump.isPressed)
        {
            velocity.y = flyingSpeed;
            rigidbody.gravityScale = 0f;
        }
        else
        {
            rigidbody.gravityScale = 1f;
        }
        
        rigidbody.linearVelocity = velocity;
    }

    private void OnDisable()
    {
        playerInputController.onPlayerInputChanged -= OnPlayerInputChanged;
    }

    void OnPlayerInputChanged(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }
}
