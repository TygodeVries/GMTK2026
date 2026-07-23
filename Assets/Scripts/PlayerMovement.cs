using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 8f;

    [SerializeField] private Transform modelTransform;
    [SerializeField] private ParticleSystem smokeSystem;

    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction eatAction;

    private Camera camera;
    private Rigidbody rb;
    private void OnEnable()
    {
        moveAction.Enable();
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
        eatAction.Enable();
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

            FindAnyObjectByType<ReturnHome>().Return();
        }

        if (spotTime > 0 && spotTime < maxSpotTime && !smokeIsPlaying)
        {
            smokeSystem.Play();
            smokeIsPlaying = true;
        }


        if (currentlyEating != null)
        {
            UpdateEat();
            return;
        }
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 worldMovement = camera.transform.rotation * new Vector3(moveInput.x, moveInput.y);
        worldMovement.y = 0;

        float speed = Mathf.Lerp(normalSpeed, 0, spotTime / maxSpotTime);
        rb.linearVelocity = worldMovement * speed;

        if (rb.linearVelocity.magnitude > 0.2f)
            modelTransform.LookAt(modelTransform.position + rb.linearVelocity.normalized);

        if (eatAction.WasPressedThisFrame())
        {
            Eat();
        }
    }

    private void UpdateEat()
    {
        Vector3 newPos = currentlyEating.transform.position - currentlyEating.transform.forward + new Vector3(0, -1, 0);
        newPos.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 2);
        modelTransform.LookAt(currentlyEating.transform.position - new Vector3(0, 1, 0));
    }
    private void Eat()
    {
        Debug.Log("Yum");
        float closest = 100000;
        Eatable[] eatables = FindObjectsByType<Eatable>();
        Eatable closestEatable = null;
        foreach (Eatable eatable in eatables)
        {
            float dis = Vector3.Distance(eatable.transform.position, transform.position);

            Debug.Log(dis);
            if (dis < closest)
            {
                closest = dis;
                closestEatable = eatable;
            }
        }

        if (closest < 2)
        {
            currentlyEating = closestEatable;
            currentlyEating.StartEat();
        }
    }

    public Eatable currentlyEating = null;

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
    }

    private float spotTime;
    [SerializeField] private float maxSpotTime;

    private bool isBeingSpotted = false;
}
