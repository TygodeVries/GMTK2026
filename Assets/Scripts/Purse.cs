using UnityEngine;

public class Purse : MonoBehaviour
{
    [SerializeField] private ParticleSystem emitCoins;
    public int goldInPurse;

    public int Clear()
    {
        int hold = goldInPurse;
        goldInPurse = 0;

        emitCoins.Emit(hold);

        return hold;
    }

    public void AddCoins(int newCoins)
    {
        goldInPurse += newCoins;
    }
}
