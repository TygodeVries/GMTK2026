using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinCount;
    private void OnCollisionEnter(Collision collision)
    {
        Purse purse = collision.gameObject.GetComponent<Purse>();

        if (purse == null)
        {
            return;
        }

        purse.AddCoins(coinCount);

        Destroy(gameObject);
    }
}
