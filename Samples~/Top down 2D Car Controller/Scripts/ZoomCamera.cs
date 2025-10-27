using UnityEngine;

// Custom data container for defining the relationship between velocity and fov.

[System.Serializable]
public struct VelocityFov
{
    public float velocity;
    public float fov;
}

// Zoom camera script
// We first obtain transform velocity by calculating its derivative from previous and current render frame.
// Because velocity can change rapidly because of framerate we use linear interpolation for smoothing.

public class ZoomCamera : MonoBehaviour
{
    public Transform cameraTarget;
    public Camera camera;
    public VelocityFov min = new VelocityFov() { fov = 66f, velocity = 1f };
    public VelocityFov max = new VelocityFov() { fov = 116f, velocity = 4f };

    private Vector3 lastTargetPosition;

    private void OnEnable()
    {
        // this will prevent sudden zoom changes when we enable the script.
        lastTargetPosition = cameraTarget.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraTargetPosition = cameraTarget.position;
        // calculate difference vector between last and current position
        Vector3 delta = cameraTargetPosition - lastTargetPosition;
        
        // divide delta magnitude by delta time to obtain velocity
        float velocity = delta.magnitude / Time.deltaTime;
        
        // convert min.velocity >= velocity <= max.velocity to range from 0 to 1 
        float fovPercent = Mathf.InverseLerp(min.velocity, max.velocity, velocity);
        
        // map our percent to our min.fov >= fov <= max.fov
        float targetFov = Mathf.Lerp(min.fov, max.fov, fovPercent);
        
        // Linear interpolation smooths out sudden changes due to frame rate fluctuations
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, Time.deltaTime);
        
        // store camera target position for future frame
        lastTargetPosition = cameraTargetPosition;
    }
}
