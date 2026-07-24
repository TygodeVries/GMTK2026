using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Collapse : MonoBehaviour
{
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

    private TileType[,,] possibilities;
    private TileType[,] map;
    private int typeCount = 0;
    private void Start()
    {
        typeCount = Enum.GetValues(typeof(TileType)).Length;
        TileType[] types = (TileType[])Enum.GetValues(typeof(TileType));

        map = new TileType[width, height];
        possibilities = new TileType[width, height, typeCount];


        // At the beginning, everything is possible
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 1; i < typeCount; i++)
                {
                    possibilities[x, y, i] = types[i];
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (map == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                TileType type = map[x, y];

                if (type == TileType.None)
                {
                    Gizmos.color = Color.red;
                }

                if (type == TileType.Land)
                {
                    Gizmos.color = Color.green;
                }

                if (type == TileType.Water)
                {
                    Gizmos.color = Color.blue;
                }

                if (type == TileType.Sand)
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawCube(new Vector3(x, 0, y), new Vector3(1, 1, 1));
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < 100; i++)
            SolveStep();
    }

    public bool SolveStep()
    {
        (int x, int y)? worst = FindWorstTile();

        if (worst == null)
            return true;

        TileType[] options = GetPossibilitiesAt(worst!.Value.x, worst.Value.y);
        TileType newType = options[UnityEngine.Random.Range(0, options.Length)];

        MakeTileAt(worst.Value.x, worst.Value.y, newType);

        return false;
    }

    public void MakeTileAt(int x, int y, TileType type)
    {
        map[x, y] = type;

        for (int i = 0; i < typeCount; i++)
            possibilities[x, y, i] = TileType.None;

        possibilities[x, y, (int)type] = type;

        UpdateNear(x + 1, y, type);
        UpdateNear(x - 1, y, type);
        UpdateNear(x, y + 1, type);
        UpdateNear(x, y - 1, type);
    }

    private void UpdateNear(int x, int y, TileType placed)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        if (map[x, y] != TileType.None)
            return;

        for (int i = 1; i < typeCount; i++)
        {
            TileType candidate = possibilities[x, y, i];

            if (candidate == TileType.None)
                continue;

            if (!CanTouch(placed, candidate))
                possibilities[x, y, i] = TileType.None;
        }
    }

    private bool CanTouch(TileType a, TileType b)
    {
        if (a == TileType.Water)
            return b == TileType.Water || b == TileType.Sand;

        if (a == TileType.Sand)
            return b == TileType.Sand || b == TileType.Water || b == TileType.Land;

        if (a == TileType.Land)
            return b == TileType.Land || b == TileType.Sand;

        return true;
    }


    private (int x, int y)? FindWorstTile()
    {
        List<(int x, int y, int count)> options = new List<(int, int, int)>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.None)
                    options.Add((x, y, GetPossibilityCount(x, y)));
            }
        }

        if (options.Count == 0)
        {
            return null;
        }

        int minCount = options.Min(tile => tile.count);

        var worstTiles = options.Where(tile => tile.count == minCount).ToList();

        var selected = worstTiles[UnityEngine.Random.Range(0, worstTiles.Count)];

        return (selected.x, selected.y);
    }

    private int GetPossibilityCount(int x, int y)
    {
        int amount = 0;

        for (int i = 0; i < typeCount; i++)
        {
            if (possibilities[x, y, i] != TileType.None)
            {
                amount++;
            }
        }
        return amount;
    }

    private TileType[] GetPossibilitiesAt(int x, int y)
    {
        List<TileType> types = new List<TileType>();

        for (int i = 0; i < typeCount; i++)
        {
            if (possibilities[x, y, i] != TileType.None)
            {
                types.Add(possibilities[x, y, i]);
            }
        }

        return types.ToArray();
    }
}

public enum TileType : byte
{
    None = 0x00,
    Sand = 0x01,
    Water = 0x02,
    Land = 0x03
}