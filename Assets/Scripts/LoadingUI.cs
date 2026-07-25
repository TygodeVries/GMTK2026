using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public void FinishLoading()
    {
        GetComponent<Animator>().SetTrigger("Done");
    }
}
