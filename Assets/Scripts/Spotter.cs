using System.Collections.Generic;
using UnityEngine;
public class Spotter : MonoBehaviour
{

    public static List<Spotter> Spotters = new List<Spotter>();
    [SerializeField] private float angle;
    [SerializeField] private float length;


    private void Start()
    {
        light = GetComponent<Light>();
    }
    private Light light;
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(origin, transform.forward * length);

        // Draw cone edges
        Vector3 left = Quaternion.AngleAxis(-angle * 0.5f, transform.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(angle * 0.5f, transform.up) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, left * length);
        Gizmos.DrawRay(origin, right * length);

        Gizmos.DrawLine((right * length) + origin, (transform.forward * length) + origin);
        Gizmos.DrawLine((left * length) + origin, (transform.forward * length) + origin);



        IsPointInCone(new Vector3(0, 0, 0));
    }

    private Transform player;
    private int frame = 0;
    private void Update()
    {
        frame++;
        // Every so often
        if (frame % 10 == 4)
        {
            // Disable lights far away
            light.enabled = Vector3.Distance(player.position, transform.position) < 40;
        }
    }

    public bool IsPointInCone(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        if (localPoint.z < 0)
            return false;

        if (localPoint.magnitude > length) // To far away
            return false;

        float pointAngle = Vector3.Angle(Vector3.forward, localPoint);

        return pointAngle <= angle * 0.5f;
    }

    private void OnEnable()
    {
        Spotters.Add(this);
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void OnDisable()
    {
        Spotters.Remove(this);
    }
}
