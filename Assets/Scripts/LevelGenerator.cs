using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buildingPrefabs = new List<GameObject>();
    [SerializeField]
    private UnityEvent OnLevelGenerationComplete;

    private void Start()
    {
        var houses = CreateHouses(60);
        OnLevelGenerationComplete.Invoke();

    }

    private List<GameObject> CreateHouses(int amount)
    {
        Coffin coffin = FindAnyObjectByType<Coffin>();

        List<GameObject> houses = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            GameObject go = GameObject.Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Count)]);
            go.transform.position = new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f));
            go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            if (Vector3.Distance(go.transform.position, coffin.transform.position) < 10)
            {
                i--;
                Destroy(go);
                continue;
            }


            Collider collider = go.GetComponent<Collider>();
            Collider[] nearbyColliders = Physics.OverlapSphere(collider.bounds.center, collider.bounds.extents.magnitude);


            bool destroyed = false;
            foreach (Collider other in nearbyColliders)
            {
                if (other == collider) continue;


                bool overlaps = Physics.ComputePenetration(
                    collider, collider.transform.position, collider.transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out Vector3 direction, out float distance
                );

                if (overlaps && distance > 0f)
                {
                    i--;
                    Destroy(go);
                    destroyed = true;
                    break;
                }
            }

            if (destroyed)
                continue;

            houses.Add(go);
        }

        return houses;
    }
}


