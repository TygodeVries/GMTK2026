using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coffin : MonoBehaviour
{
    public int score;
    [SerializeField] private Animator animator;
    private Purse player;

    private int maxScore = 0;
    private void Start()
    {
        player = FindAnyObjectByType<Purse>();
    }

    // Called by external script
    public void OnLevelReady()
    {
        maxScore = 10;
        UpdateText();


    }

    private void Update()
    {
        bool isNear = Vector3.Distance(player.transform.position, gameObject.transform.position) < 4;
        animator.SetBool("IsOpen", isNear && player.goldInPurse != 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;

        Purse purse = collision.gameObject.GetComponent<Purse>();
        score += purse.Clear();
        UpdateText();

        if (score >= maxScore)
        {
            StartCoroutine(StartNext());
        }
    }

    private IEnumerator StartNext()
    {
        FindAnyObjectByType<LoadingUI>().GetComponent<Animator>().SetTrigger("Again");

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(3);
    }

    private void UpdateText()
    {
        GetComponentInChildren<TMP_Text>().text = $"{score}/{maxScore}";
    }
}
