using UnityEngine;

public class Coffin : MonoBehaviour
{
    public int score;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;

        Purse purse = collision.gameObject.GetComponent<Purse>();
        score += purse.Clear();
    }
}
