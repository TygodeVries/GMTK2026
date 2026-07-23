using UnityEngine;

public class Floor : MonoBehaviour
{
    public void Start()
    {
        GameObject pre = transform.GetChild(0).gameObject;

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                if (x == 0 && z == 0)
                    continue;
                GameObject go = GameObject.Instantiate(pre, new Vector3(x * 10, 0, z * 10), Quaternion.identity);
                go.transform.parent = transform;
            }
        }
    }
}
