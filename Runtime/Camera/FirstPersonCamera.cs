using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform target;
    public float mouseSensitivity = 1f;
    public bool flipMouseY = true;
    public float cameraYaw = 0;
    public float cameraPitch = 0;
    
    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        cameraYaw += mouseX * mouseSensitivity * 10f;
        
        float mouseY = Input.GetAxis("Mouse Y");
        if (flipMouseY)
            mouseY *= -1f;

        cameraPitch = Mathf.Clamp(cameraPitch + mouseY * 10f * mouseSensitivity, -90f, 90f);

        transform.position = target.position;
        transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }
}
