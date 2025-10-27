using UnityEngine;
using UnityEngine.InputSystem;

// Simplest form of very old-school walking controller which
// uses left and right input for player rotation and
// forward and back for moving forward or backwards.

public class WalkingMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    public Rigidbody2D rigidbody2D;
    public float speed = 1f;
    public float rotationSpeed = 1f;

    void Start()
    {
        moveAction.action.Enable();
    }

    private void FixedUpdate()
    {
        Vector2 movement = moveAction.action.ReadValue<Vector2>();
        rigidbody2D.rotation -= rotationSpeed * movement.x;
            
        float radians = rigidbody2D.rotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        rigidbody2D.linearVelocity = direction * speed * movement.y;
    }
}
