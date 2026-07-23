using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int world_size = 100;

    public Object civilian_woman;
    public Object civilian_man;
    public Object cop;

    Vector2[,] velocity;
    List<Boid>[,] boids_per_tile;
    void Start()
    {
        velocity = new Vector2[world_size, world_size];
        boids_per_tile = new List<Boid>[world_size, world_size];

        for (int cx = 0; cx < 10; cx++)
        {
            GameObject.Instantiate(civilian_man, new Vector3(Random.Range(0, world_size), 0, Random.Range(0, world_size)), Quaternion.identity);
            GameObject.Instantiate(civilian_woman, new Vector3(Random.Range(0, world_size),0, Random.Range(0, world_size)), Quaternion.identity);
        }
        for (int cx = 0; cx < 10; cx++)
            GameObject.Instantiate(cop, new Vector3(Random.Range(0, world_size), 0 , Random.Range(0, world_size)), Quaternion.identity);
    }
    public Vector2 GetTargetVelocity(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        if (x >= 0 && x < world_size && y >= 0 && y < world_size)
        {
            float fx = position.x - x;
            float fy = position.y - y;
            int xo = Mathf.Min(x + 1, world_size - 1);
            int yo = Mathf.Min(y + 1, world_size - 1);
            float ofx = 1f - fx;
            float ofy = 1f - fy;
            

            return 
                velocity[x, y] * ofx * ofy +
                velocity[x, yo] * ofx * fy +
                velocity[xo, y] * fx * ofy +
                velocity[xo, yo] * fx * fy;
        }
        else
            return new Vector2(0, 0);
    }
    public Vector2 BumpWithWorld(Vector2 position, bool bVel = true)
    {
        float offset = 2f;
        if (bVel)
            offset += 0.1f;
        Vector2 new_position = position;
        if (position.x < offset)
            new_position.x = offset;
        if (position.x > world_size - offset)
            new_position.x = world_size - offset;
        if (position.y < offset) 
              new_position.y = offset;
        if (position.y > world_size - offset)
                new_position.y = world_size - offset;

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
    public List<Boid> getNearBoids(Vector2 p)
    {
        int x = (int)p.x / 10;
        int y = (int)p.y / 10;
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

        float[,] count = new float[world_size, world_size];
        Boid[] boids = Object.FindObjectsByType<Boid>();
        foreach (Boid b in boids)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {

                    int x = (int)b.position.x / 10 + dx;
                    int y = (int)b.position.y / 10 + dy;

                    if (x >= 0 && x < world_size && y >= 0 && y < world_size)
                    {
                        boids_per_tile[x, y].Add(b);
                        velocity[x, y] += b.velocity * b.speed_weight;
                        count[x, y] += Mathf.Abs(b.speed_weight);
                    }
                }
        }
        for (int x = 0; x < world_size; x++)
            for (int y = 0; y < world_size; y++)
                if (count[x, y] > 0)
                    velocity[x, y] /= count[x, y];
    }
}
