using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 8f;

    [SerializeField] private Transform modelTransform;
    [SerializeField] private ParticleSystem smokeSystem;

    [SerializeField] private InputAction moveAction;

    private Camera camera;
    private Rigidbody rb;
    private void OnEnable()
    {
        moveAction.Enable();
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
    }


    private bool smokeIsPlaying = false;
    private void Update()
    {
        CheckSpotting();

        if (spotTime > 0)
            spotTime -= Time.deltaTime;

        if (spotTime < 0 && smokeIsPlaying)
        {
            smokeIsPlaying = false;
            smokeSystem.Stop();
        }

        if (spotTime > maxSpotTime && smokeIsPlaying)
        {
            smokeIsPlaying = false;
            smokeSystem.Stop();
        }

        if (spotTime > 0 && spotTime < maxSpotTime && !smokeIsPlaying)
        {
            smokeSystem.Play();
            smokeIsPlaying = true;
        }
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 worldMovement = camera.transform.rotation * new Vector3(moveInput.x, moveInput.y);
        worldMovement.y = 0;

        float speed = Mathf.Lerp(normalSpeed, 0, spotTime / maxSpotTime);
        rb.linearVelocity = worldMovement * speed;

        if (rb.linearVelocity.magnitude > 0.2f)
            modelTransform.LookAt(modelTransform.position + rb.linearVelocity.normalized);
    }

    private void CheckSpotting()
    {
        isBeingSpotted = false;
        foreach (Spotter spotter in Spotter.Spotters)
        {
            if (spotter.IsPointInCone(transform.position))
            {
                isBeingSpotted = true;
            }
        }

        if (isBeingSpotted)
        {
            spotTime += Time.deltaTime * 2;
        }

        Debug.Log(spotTime);
    }

    private float spotTime;
    [SerializeField] private float maxSpotTime;

    private bool isBeingSpotted = false;
}
