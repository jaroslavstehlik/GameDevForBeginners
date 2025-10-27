using System;
using UnityEngine;

public class SetCameraTarget : MonoBehaviour
{
    public Transform cameraTargetTransform;
    public bool setOnEnable = false;

    private void OnEnable()
    {
        if (setOnEnable)
        {
            Set(cameraTargetTransform);
        }
    }

    public void Set(Transform cameraTarget)
    {
        Camera camera = Camera.main;
        if(camera == null)
            return;

        CameraTargetController cameraTargetController = camera.GetComponentInChildren<CameraTargetController>();
        if(cameraTargetController == null)
            return;
        
        cameraTargetController.SetTarget(cameraTargetTransform);
    }
}
