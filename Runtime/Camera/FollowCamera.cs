using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 10f, 0f);
    public float speed = 0.5f;
    
    private void LateUpdate()
    {
        transform.position = LerpUtils.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
    }
}
