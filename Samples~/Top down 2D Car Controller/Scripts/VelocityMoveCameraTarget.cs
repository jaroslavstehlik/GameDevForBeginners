using UnityEngine;

// VelocityMoveCameraTarget is for moving target by its transform velocity
// The faster the transform moves, the further it places the target.
// This is useful for camera looking in front of the moving object, instead of behind it. 

public class VelocityMoveCameraTarget : MonoBehaviour
{
    public Transform cameraTarget;
    public float amount = 100f;
    public float speed = 10f;
    public float clampMagnitude = 10f;
    private Vector3 lastTargetPosition;

    private Vector3 velocity = Vector3.zero;
    private void OnEnable()
    {
        velocity = Vector3.zero;
        lastTargetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;
        velocity = transform.InverseTransformDirection(position - lastTargetPosition) / Time.fixedDeltaTime;
        lastTargetPosition = position;
    }

    private void Update()
    {
        cameraTarget.localPosition =
            Vector3.Lerp(cameraTarget.localPosition, velocity * amount, Time.deltaTime * speed);
    }
}
