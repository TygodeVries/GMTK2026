using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int world_size = 100;

    public Object civilian_blueprint;

    Vector2[,] velocity;
    List<Boid>[,] boids_per_tile;
    void Start()
    {
        velocity = new Vector2[world_size, world_size];
        boids_per_tile = new List<Boid>[world_size, world_size];

        for (int cx = 0; cx < 100; cx ++)
            GameObject.Instantiate(civilian_blueprint, new Vector3(Random.Range(0, world_size), Random.Range(0, world_size), 0), Quaternion.identity);
    }
    public Vector2 GetTargetVelocity(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        if (x >= 0 && x < world_size && y >= 0 && y < world_size)
            return velocity[x, y];
        else
            return new Vector2(0, 0);
    }
    public Vector2 BumpWithWorld(Vector2 position)
    {
        Vector2 new_position = position;
        if (position.x < 0.2f)
            new_position.x = 0.2f;
        if (position.x > world_size - 0.2f)
            new_position.x = world_size - 0.2f;
        if (position.y < 0.2f) 
              new_position.y = 0.2f;
        if (position.y > world_size - 0.2f)
                new_position.y = world_size - 0.2f;

        // Get all BuildingBoundingBox objects
        // transform the current position to the BBB space
        // Project out of the BBB, reproject to world space
       BuildingBoundingBox[] bbb_objects = Object.FindObjectsByType<BuildingBoundingBox>();

        foreach (BuildingBoundingBox bbb in bbb_objects)
        {
            new_position = bbb.Reproject(new_position);
        }

        return new_position;
    }
    public List<Boid> getNearBoids(int x, int y)
    {
        List<Boid> near_boids = new List<Boid>();
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < world_size && ny >= 0 && ny < world_size)
                    near_boids.AddRange(boids_per_tile[nx, ny]);
            }
        return near_boids;
    }
    // Update is called once per frame
    void Update()
    {
        for (int x = 0; x < world_size; x++)
            for (int y = 0; y < world_size; y++)
            {
                velocity[x, y] = new Vector2(0, 0);
                boids_per_tile[x, y] = new List<Boid>();
            }

        int[,] count = new int[world_size, world_size];
        Boid[] boids = Object.FindObjectsByType<Boid>();
        foreach (Boid b in boids)
        {
            int x = (int)b.position.x;
            int y = (int)b.position.y;

            if (x >= 0 && x < world_size && y >= 0 && y < world_size)
            {
                boids_per_tile[x, y].Add(b);
                velocity[x, y] += b.velocity;
                count[x, y]++;
            }
            else // Boid is off grid...
                GameObject.Destroy(b.gameObject);
        }
        for (int x = 0; x < world_size; x++)
            for (int y = 0; y < world_size; y++)
                if (count[x, y] > 0)
                    velocity[x, y] /= count[x, y];
    }
}
