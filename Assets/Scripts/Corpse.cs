using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public static List<Corpse> activeCorpses = new List<Corpse>();

    private void OnEnable()
    {
        activeCorpses.Add(this);
        Debug.Log($"Corpses active: {activeCorpses.Count}");
    }

    private void OnDisable()
    {
        activeCorpses.Remove(this);
        Debug.Log($"Corpses active: {activeCorpses.Count}");
    }
}
