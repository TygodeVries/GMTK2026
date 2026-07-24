using UnityEngine;

public class ScreamSpotter : Spotter
{
    [SerializeField] private GameObject screamObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public bool isScreaming = false;
    float screamTime = 0;
    // Update is called once per frame
    void Update()
    {
        Transform player = Object.FindAnyObjectByType<PlayerMovement>().transform;

        if (IsPointInCone(player.position))
        {
            Debug.Log("AAAAAHHHHHH!");
            screamObject.SetActive(true);
            isScreaming = true;
            screamTime = 10;
        }
        else
        {
            screamTime = Mathf.Max(0, screamTime - Time.deltaTime);
            isScreaming = screamTime > 0;
            screamObject.SetActive(isScreaming);
        }
    }

    public static void SetEnabled(bool enabled)
    {
        foreach (ScreamSpotter screamer in FindObjectsByType<ScreamSpotter>())
        {
            screamer.enabled = enabled;
        }

    }
}
