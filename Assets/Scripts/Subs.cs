using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Subs : MonoBehaviour
{
    [SerializeField]
    private List<Subtitle> subtitles;
    private TMP_Text tmp;

    public float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        tmp = GetComponent<TMP_Text>();
        int ind = 0;
        while (ind != subtitles.Count)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;

            if (timer > subtitles[ind].time)
            {
                Debug.Log("Next: " + subtitles[ind].text);
                tmp.text = subtitles[ind].text;
                ind++;
            }
        }
    }
}

[Serializable]
public class Subtitle
{
    public string text;
    public float time;
}
