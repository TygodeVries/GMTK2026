using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assemblies;

public class Boid : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        position = new Vector2(transform.position.x, transform.position.z ); // Needs to get worldsize
    }
    public Vector2 velocity;
    public Vector2 position;
    public float minimize_distance = 0.25f;
    public float speed = .25f;
    public float speed_weight = 1f;
    // Update is called once per frame
    void LateUpdate()
    {
        WorldManager worldManager = Object.FindAnyObjectByType<WorldManager>();
        
        Vector2 target_velocity = worldManager.GetTargetVelocity(position);
        Vector2 target_position = position + target_velocity / 10.0f;
        bool iter = false;
        int iter_count = 5;
        do
        {
            iter = false;
            target_position = worldManager.BumpWithWorld(target_position);
            foreach (Boid b in worldManager.getNearBoids(position))
                if (b != this)
                {
                    Vector2 diff = b.position - target_position;
                    float distance = diff.magnitude;
                    if (distance < minimize_distance && distance > 0.0f)
                    {
                        target_position -= diff.normalized * (minimize_distance - distance) * 0.9f;
                        iter = true;
                    }
                }
        }
        while (iter && iter_count --> 0);

        if ((target_position - position).magnitude > 0.0001f)
        {
            Vector2 tmp_velocity = (target_position - position).normalized;
            velocity = 0.9f * velocity + 0.1f * tmp_velocity;
            velocity = velocity.normalized;
        }
        if (GetComponent<Eatable>() is Eatable eatable && eatable.eaten)
        {
            velocity = Vector2.zero;
        }

        position += velocity * Time.deltaTime * speed;
        position = worldManager.BumpWithWorld(position, false);
        transform.position = new Vector3(position.x, 0, position.y);
        transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.y));
    }
}
