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

        transform.position = targetTransform.position + offset;
    }
}
