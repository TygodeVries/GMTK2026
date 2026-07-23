using UnityEngine;

public class RandomColor : MonoBehaviour
{

    private Material mat;
    public void Start()
    {
        mat = GameObject.Instantiate(GetComponentInChildren<MeshRenderer>().material);
        mat.SetColor("_Hair_Color", GetRandomColor());
        mat.SetColor("_Shirt_Color", GetRandomColor());
        mat.SetColor("_Pants_Color", GetRandomColor());
        mat.SetColor("_Dress_Color", GetRandomColor());
        mat.SetColor("_Hands_Color", GetRandomColor());
        mat.SetColor("_Cloak_Color", GetRandomColor());
        GetComponentInChildren<MeshRenderer>().material = mat;
    }

    private Color GetRandomColor()
    {
        return Color.HSVToRGB(Random.Range(0, 1f), 45f / 100f, 80f / 100f);
    }


    private void OnDisable()
    {
        Destroy(mat);
    }

    private void OnDestroy()
    {
        Destroy(mat);
    }
}
