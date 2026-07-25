using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds((1 * 60) + 16);
        SceneManager.LoadScene(2);
    }
}
