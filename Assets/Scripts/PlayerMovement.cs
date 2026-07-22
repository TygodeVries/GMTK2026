using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float topSpeed = 8f;
    [SerializeField] private float timeToTopSpeed = 1.5f;


    [SerializeField] private InputAction moveAction;

    private Camera camera;
    private Rigidbody rb;
    private void OnEnable()
    {
        moveAction.Enable();
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 worldMovement = camera.transform.rotation * new Vector3(moveInput.x, moveInput.y);
        worldMovement.y = 0;

        rb.linearVelocity = worldMovement * topSpeed;
    }
}
