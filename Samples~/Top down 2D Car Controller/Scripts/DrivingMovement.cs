using UnityEngine;
using UnityEngine.InputSystem;

// Driving movement, Controller for car driving behaviour
// Script listens to controller input and moves rigidbody in a car like physics.
// Car controllers are difficult because we want to simulate tire physics.
// In 2D physics we cannot properly simulate top down can physics, because
// the car does not have any tires which would touch the road and produce friction.
// Therefore we calculate the car movement manually instead of relying on the physics engine.
// And stop the car when we hit collider or other rigidbody.

public class DrivingMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    public Rigidbody2D rigidbody2D;
    public float acceleration = 0.5f;
    public float maxSpeed = 50f;
    public float rotationSpeed = 0.1f;
    
    private float _currentSpeed = 0f;
    private Vector2 _movementInput = Vector2.zero;
    private const int MAX_CONTACT_POINTS = 10;
    
    private ContactPoint2D[] _contactPoints = new ContactPoint2D[MAX_CONTACT_POINTS];
    private int _totalContactPoints = 0;
    [SerializeField] private bool _debugPhysics = false;
    
    void Start()
    {
        moveAction.action.Enable();
    }

    private void OnEnable()
    {
        _currentSpeed = 0f;
    }

    private void Update()
    {
        _movementInput = moveAction.action.ReadValue<Vector2>();
    }

    Vector2 GetDirectionFromAngle(float angle)
    {
        float angleRadians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
    }

    private void FixedUpdate()
    {
        // create a direction vector from rotation angle
        Vector2 currentDirection = GetDirectionFromAngle(rigidbody2D.rotation);
        // Extract driving velocity in the orientation of the car
        float currentDriveVelocity = Vector2.Dot(currentDirection, rigidbody2D.linearVelocity);
        // clamp speed based on min and max speed;
        _currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * _movementInput.y, -maxSpeed, maxSpeed);
        // update future angle based on input and car velocity
        // the slower the car moves the slower it rotates and vice versa
        float futureAngle = rigidbody2D.rotation - rotationSpeed * _movementInput.x * currentDriveVelocity;
        // obtain future car direction
        Vector2 futureDirection = GetDirectionFromAngle(futureAngle);
        // obtain future car velocity by multiplying direction and speed
        Vector2 futureVelocity = futureDirection * _currentSpeed;
        
        // Get all collision points
        _totalContactPoints = rigidbody2D.GetContacts(new ContactFilter2D()
        {
            useTriggers = false,
        }, _contactPoints);

        // Iterate every contact point and obtain total impact velocity
        Vector2 hitVelocity = TotalContactVelocity(_totalContactPoints, _contactPoints);
        
        // Isolate relative velocity to our car rotation
        float relativeHitVelocity = Vector2.Dot(futureDirection, hitVelocity);
        
        // Apply impact velocity to our speed
        _currentSpeed += relativeHitVelocity * Time.unscaledDeltaTime;
        
        // deaccelerate when we release throttle
        if (Mathf.Abs(_movementInput.y) < 0.1f)
        {
            _currentSpeed /= 1f + rigidbody2D.linearDamping * 0.01f;
        }

        // move car based on velocity
        rigidbody2D.linearVelocity = futureVelocity;
        // rotate car based on movement input, car speed and rotation speed
        rigidbody2D.rotation = futureAngle;
    }
    
    static Vector2 TotalContactVelocity(int totalContactPoints, ContactPoint2D[] contactPoints)
    {
        Vector2 velocity = Vector2.zero;
        for (int i = 0; i < totalContactPoints; i++)
        {
            float otherMass = 10000f;
            if (contactPoints[i].rigidbody != null)
            {
                otherMass = contactPoints[i].rigidbody.mass;
            }

            Rigidbody2D currentRigidbody = contactPoints[i].otherRigidbody; 
            float mass = currentRigidbody.mass;
            float totalMass = mass + otherMass;
            Vector2 totalVelocity = contactPoints[i].relativeVelocity * totalMass;
            Vector2 myVelocity = totalVelocity / mass;
            velocity += myVelocity;
        }
        
        return velocity;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!_debugPhysics) 
            return;
        
        for (int i = 0; i < _totalContactPoints; i++)
        {
            Gizmos.DrawWireSphere(_contactPoints[i].point, 0.1f);
            Gizmos.DrawLine(_contactPoints[i].point, _contactPoints[i].point + _contactPoints[i].normal);
        }
    }
    #endif
}
