using UnityEngine;

/*
 Walking is the simplest movement a player can do.
 We need to just change the horizontal velocity of the player.
 
 To have proper collision detection we have to change velocity of rigidbody.
 If we would change the position instead, player could walk through walls because: 
 changing position is considered teleportation, 
 while changing velocity tells the physics engine where we want to be in the future
 and therefore it can react to collisions properly. 
 */

public class PlayerWalking : MonoBehaviour
{
    public PlayerInputController playerInputController;
    public Rigidbody2D rigidbody;
    public float moveSpeed = 1f;
    
    private PlayerInput _playerInput = new PlayerInput();
    
    private void OnEnable()
    {
        playerInputController.onPlayerInputChanged += OnPlayerInputChanged;
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rigidbody.linearVelocity;
        velocity.x = _playerInput.move.x * moveSpeed;
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
