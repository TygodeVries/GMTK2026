using UnityEngine;

public class AAAH : MonoBehaviour
{
    [SerializeField] private Material ahhMaterial;

    private float timer = 0;
    private Material materialInstance;

    private void Start()
    {
        materialInstance = Instantiate(ahhMaterial);
        GetComponent<MeshRenderer>().material = materialInstance;
    }

    private void Update()
    {
        timer += Time.deltaTime * (timer + 3);
        materialInstance.SetFloat("_Size", timer);
    }
}
