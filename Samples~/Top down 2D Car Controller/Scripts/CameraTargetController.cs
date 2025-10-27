using UnityEngine;

// Camera behaviour agnostic camera target controller
// The agnosticism is achieved by using a camera transform target
// which is being reparented to a parent which wants to become the target.
// This makes the script unaware of any other scripts.

public class CameraTargetController : MonoBehaviour
{
    // this is our camera target which is reparented
    public Transform cameraTargetTransform;

    // Call this to reparent our camera target to a different parent
    public void SetTarget(Transform transform)
    {
        // reparent our camera target transform to a new parent
        cameraTargetTransform.SetParent(transform);
        
        // reset transformations after reparenting
        cameraTargetTransform.localPosition = Vector3.zero;
        cameraTargetTransform.localRotation = Quaternion.identity;
        cameraTargetTransform.localScale = Vector3.one;
    }
}
