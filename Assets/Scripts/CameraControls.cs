using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private Vector3 offset;
    public Transform target;
    private float smoothSpeed = 0.25f;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position;
        targetPosition.x += offset.x;
        targetPosition.y += offset.y;
        targetPosition.z += offset.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothSpeed);
    }
}