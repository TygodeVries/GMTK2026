using TMPro;
using UnityEngine;

public class Coffin : MonoBehaviour
{
    public int score;
    [SerializeField] private Animator animator;
    private Purse player;

    private void Start()
    {
        player = FindAnyObjectByType<Purse>();
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
        GetComponentInChildren<TMP_Text>().text = $"{score}/200";
    }
}
