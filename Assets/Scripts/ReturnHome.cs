using System.Collections;
using UnityEngine;

public class ReturnHome : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Return()
    {
        StartCoroutine(Do());
    }

    private IEnumerator Do()
    {

        int gold = FindAnyObjectByType<Purse>().goldInPurse;

        transform.GetChild(0).gameObject.SetActive(true);
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        Coffin coffin = FindAnyObjectByType<Coffin>();
        transform.position = player.transform.position;
        Vector3 startPos = player.transform.position;
        Vector3 endPos = coffin.transform.position + new Vector3(2, 0, 0);
        // Set to one so the animation plays, #hack
        FindAnyObjectByType<Purse>().goldInPurse = 1;
        FindAnyObjectByType<CameraController>().targetTransform = transform;
        GetComponent<ParticleSystem>().Emit(50);

        for (int i = 0; i < gold; i++)
        {
            GameObject go = GameObject.Instantiate(coinPrefab);
            Rigidbody body = go.GetComponent<Rigidbody>();
            body.transform.position = transform.position + new Vector3(0, 2, 0);
            body.linearVelocity = new Vector3(Random.Range(-4, 4), 5, Random.Range(-4, 4));
        }

        yield return new WaitForSeconds(0.5f);

        player.transform.position = coffin.transform.position - new Vector3(0, 3, 0);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(startPos, endPos, EaseOutCirc(t));
        }
        transform.position = endPos;
        FindAnyObjectByType<CameraController>().targetTransform = player.transform;
        player.transform.position = endPos;
        FindAnyObjectByType<Purse>().goldInPurse = 0;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    private float EaseOutCirc(float x)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
    }
}
