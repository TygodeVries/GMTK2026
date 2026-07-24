using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Collapse : MonoBehaviour
{
    [SerializeField] public int width = 20;
    [SerializeField] public int height = 20;

    private TileType[,,] possibilities;
    private TileType[,] map;

    public TileType GetTileAt(int x, int y)
    {
        return map[x, y];
    }

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
                for (int i = 2; i < typeCount; i++)
                {
                    possibilities[x, y, i] = types[i];
                }

                possibilities[x, y, (int)TileType.Plaza] = TileType.None;
                possibilities[x, y, (int)TileType.House] = TileType.None;
            }
        }
    }

    [SerializeField] private UnityEvent OnComplete;

    private bool completed = false;
    private void Update()
    {
        if (completed) return;

        for (int i = 0; i < 100; i++)
        {
            if (SolveStep())
            {
                OnComplete.Invoke();
                completed = true;
                return;
            }
        }
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

        if (type == TileType.XStreet)
        {
            NoRoads(x, y + 1);
            NoRoads(x, y - 1);

            RoadEnd(x + 1, y, type);
            RoadEnd(x - 1, y, type);
        }

        if (type == TileType.YStreet)
        {
            NoRoads(x + 1, y);
            NoRoads(x - 1, y);

            RoadEnd(x, y + 1, type);
            RoadEnd(x, y - 1, type);
        }

        if (type == TileType.Plaza)
        {
            ForbiddenPlaza(x + 1, y);
            ForbiddenPlaza(x - 1, y);
            ForbiddenPlaza(x, y + 1);
            ForbiddenPlaza(x, y - 1);
        }
    }

    public void ForbiddenPlaza(int x, int y)
    {
        if (x == -1 || x == width)
            return;

        if (y == -1 || y == height)
            return;

        possibilities[x, y, (int)TileType.Plaza] = TileType.Forbidden;
    }

    public void NoRoads(int x, int y)
    {
        if (x == -1 || x == width)
            return;

        if (y == -1 || y == height)
            return;

        possibilities[x, y, (int)TileType.YStreet] = TileType.None;
        possibilities[x, y, (int)TileType.XStreet] = TileType.None;

        possibilities[x, y, (int)TileType.House] = TileType.House;
        possibilities[x, y, (int)TileType.House] = TileType.House;

        ForbiddenPlaza(x, y);
        ForbiddenPlaza(x, y);
    }

    public void RoadEnd(int x, int y, TileType me)
    {
        if (x == -1 || x == width)
            return;

        if (y == -1 || y == height)
            return;

        // Allow

        if (possibilities[x, y, (int)TileType.Plaza] != TileType.Forbidden)
            possibilities[x, y, (int)TileType.Plaza] = TileType.Plaza;

        // Deny
        if (me == TileType.XStreet)
        {
            possibilities[x, y, (int)TileType.YStreet] = TileType.None;
        }

        if (me == TileType.YStreet)
        {
            possibilities[x, y, (int)TileType.XStreet] = TileType.None;
        }

        possibilities[x, y, (int)TileType.Grass] = TileType.None;
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
    Forbidden = 0x10,

    Grass = 0x01,
    YStreet = 0x02,
    XStreet = 0x03,
    Plaza = 0x04,
    House = 0x05

}