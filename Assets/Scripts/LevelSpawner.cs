using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField]
    private List<TypeMap> maps;
    private Collapse Collapse;

    private void Start()
    {
        Collapse = GetComponent<Collapse>();
    }

    public void Generate()
    {
        for (int x = 0; x < Collapse.width; x++)
        {
            for (int y = 0; y < Collapse.height; y++)
            {
                TileType type = Collapse.GetTileAt(x, y);
                TypeMap map = maps.FirstOrDefault(o =>
                {
                    return o.type == type;
                });

                if (map == null)
                {
                    Debug.LogError("No prefab for tile type: " + type);
                    continue;
                }

                // Pick a random prefab
                GameObject o = GameObject.Instantiate(map.prefabs[UnityEngine.Random.Range(0, map.prefabs.Count)]);
                o.transform.position = new Vector3((x * 5) + 2.5f, 0, (y * 5) + 2.5f);
            }
        }
    }
}

[Serializable]
public class TypeMap
{
    public TileType type;
    public List<GameObject> prefabs;
}
