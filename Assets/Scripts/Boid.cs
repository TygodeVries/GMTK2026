using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assemblies;

public class Boid : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        position = new Vector2(Random.Range(0.0f, 10.0f), Random.Range(0.0f, 10.0f)); // Needs to get worldsize

        debug_lines = new List<(Vector3, Vector3)>();
    }
    public Vector2 velocity;
    public Vector2 position;
    public float minimize_distance = 0.25f;
    public float speed = .25f;
    List<(Vector3, Vector3)> debug_lines = new List<(Vector3, Vector3)>();
    // Update is called once per frame
    void LateUpdate()
    {
        WorldManager worldManager = Object.FindAnyObjectByType<WorldManager>();
        
        Vector2 target_velocity = worldManager.GetTargetVelocity(position);
        Vector2 target_position = position + target_velocity / 10.0f;
        int x = (int)position.x;
        int y = (int)position.y;
        bool iter = false;
        int iter_count = 5;
        debug_lines.Clear();
        do
        {
            iter = false;
            target_position = worldManager.BumpWithWorld(target_position);
            foreach (Boid b in worldManager.getNearBoids(x, y))
                if (b != this)
                {
                    Vector2 diff = b.position - target_position;
                    debug_lines.Add((new Vector3(target_position.x, 0.05f, target_position.y), new Vector3(b.position.x, 0.05f, b.position.y)));
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
        position += velocity * Time.deltaTime * speed;
        position = worldManager.BumpWithWorld(position);
        transform.position = new Vector3(position.x, 0, position.y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach(var line in debug_lines)
            Gizmos.DrawLine(line.Item1, line.Item2);
    }
}
