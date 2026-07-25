using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BrightnessSlider : MonoBehaviour
{

    [SerializeField] private Slider slider;

    private ColorAdjustments ca;
    public void Start()
    {
        ca = null;
        GetComponent<Volume>().profile.TryGet(out ca);

        if (ca == null)
            return;

        ca.postExposure.value = PlayerPrefs.GetFloat("Brightness");

        if (slider != null)
        {
            slider.value = PlayerPrefs.GetFloat("Brightness");
        }
    }

    public void OnSliderChange()
    {
        ca.postExposure.value = slider.value;
        PlayerPrefs.SetFloat("Brightness", ca.postExposure.value);
        PlayerPrefs.Save();
    }

    public void NextScene()
    {
        PlayerPrefs.Save();
        FindAnyObjectByType<Animator>().SetTrigger("Exit");
        StartCoroutine(LoadNextSceneLater());
    }

    private IEnumerator LoadNextSceneLater()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}
