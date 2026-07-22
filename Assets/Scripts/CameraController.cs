using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    private void Update()
    {
        if (playerTransform == null)
            return;

        transform.position = playerTransform.position + offset;
    }
}
