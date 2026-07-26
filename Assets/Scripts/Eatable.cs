using System.Collections;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    [SerializeField] private GameObject censorPrefab;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount;

    public bool eaten;

    public void StartEat()
    {
        eaten = true;
        StartCoroutine(Eat());
    }

    private IEnumerator Eat()
    {
        gameObject.AddComponent<Corpse>();

        // Start eating animation
        GameObject censorObject = GameObject.Instantiate(censorPrefab, gameObject.transform);
        //        censorObject.transform.position = gameObject.transform.position;

        yield return new WaitForSeconds(4);
        // Spawn some gold

        FindAnyObjectByType<PlayerMovement>().currentlyEating = null;

        if (transform.GetChild(0).GetComponent<Spotter>() != null)
        {
            Destroy(
            transform.GetChild(0).gameObject
            );
        }

        for (int i = 0; i < coinAmount; i++)
        {
            GameObject go = GameObject.Instantiate(coinPrefab);
            Rigidbody body = go.GetComponent<Rigidbody>();
            body.transform.position = transform.position + new Vector3(0, 2, 0);
            body.linearVelocity = new Vector3(Random.Range(-4, 4), 5, Random.Range(-4, 4));
        }
    }
}
