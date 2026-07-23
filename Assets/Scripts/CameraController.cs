using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] public Transform targetTransform;
    [SerializeField] private Vector3 offset;
    private void Update()
    {
        if (targetTransform == null)
            return;

        transform.position = Vector3.Lerp(transform.position, targetTransform.position + offset, Time.deltaTime * 4);
    }
}
