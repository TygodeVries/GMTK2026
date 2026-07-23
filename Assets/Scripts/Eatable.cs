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
        yield return new WaitForSeconds(1);
        GameObject censorObject = GameObject.Instantiate(censorPrefab);

        censorObject.transform.position = transform.position;

        yield return new WaitForSeconds(3);
        FindAnyObjectByType<PlayerMovement>().currentlyEating = null;

        GetComponentInChildren<ParticleSystem>().Play();
        Destroy(
        transform.GetChild(0).gameObject
        );
    }
}
