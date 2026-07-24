using System.Collections;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    [SerializeField] private GameObject censorPrefab;

    public bool eaten;

    public void StartEat()
    {
        eaten = true;
        StartCoroutine(Eat());
    }

    private IEnumerator Eat()
    {
        // Start eating animation
        GameObject censorObject = GameObject.Instantiate(censorPrefab);

        censorObject.transform.position = transform.position;

        yield return new WaitForSeconds(4);

        // Spawn some gold

        FindAnyObjectByType<PlayerMovement>().currentlyEating = null;

        if (transform.GetChild(0).GetComponent<Spotter>() != null)
        {
            Destroy(
            transform.GetChild(0).gameObject
            );
        }

        Destroy(censorObject, 10);
        Destroy(this.gameObject, 9);
    }
}
