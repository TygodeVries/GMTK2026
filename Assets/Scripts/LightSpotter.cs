using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LightSpotter : Spotter
{
    public static List<Spotter> Spotters = new List<Spotter>();

    private void Start()
    {
        light = GetComponent<Light>();
    }
    private Light light;


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
