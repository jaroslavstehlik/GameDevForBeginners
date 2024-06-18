using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector2 minMaxDistance = new Vector2(0.25f, 5f);
    public float mouseSensitivity = 1f;
    public float mouseScrollWheelSensitivity = 1f;
    public bool flipMouseY = true;
    public float cameraYaw = 0;
    public float cameraPitch = 0;
    public float cameraDistance = 10f;
    public LayerMask cameraCollisionMask;
    public float cameraRadius = 0.1f;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        cameraYaw += mouseX * mouseSensitivity * 10f;
        
        float mouseY = Input.GetAxis("Mouse Y");
        if (flipMouseY)
            mouseY *= -1f;

        // clamp horizon
        cameraPitch = Mathf.Clamp(cameraPitch + mouseY * 10f * mouseSensitivity, -90, 90);
        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        
        Vector3 targetPosition = target.position;
        Vector3 targetDirection = rotation * Vector3.forward;

        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        cameraDistance = Mathf.Clamp(cameraDistance + mouseScrollWheel * mouseScrollWheelSensitivity, minMaxDistance.x, minMaxDistance.y);
        
        Ray sphereCastRay = new Ray(targetPosition, -targetDirection);
        RaycastHit raycastHit;
        bool hit = Physics.SphereCast(sphereCastRay, cameraRadius, out raycastHit, minMaxDistance.y, cameraCollisionMask,
            QueryTriggerInteraction.Ignore);

        float actualCameraDistance = Vector3.Distance(targetPosition, transform.position);
        float cameraDistanceWithPhysics = Mathf.Lerp(actualCameraDistance, cameraDistance, Lerp.Smooth(0.1f));
        if (hit)
        {
            cameraDistanceWithPhysics = Mathf.Min(cameraDistance, raycastHit.distance);
        }
        
        transform.position = targetPosition - targetDirection * cameraDistanceWithPhysics;
        transform.rotation = rotation;
    }
}
