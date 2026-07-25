using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using System.Numerics;
using Unity.VisualScripting.Antlr3.Runtime;

public class Collapse : MonoBehaviour
{
    [SerializeField] public int width = 20;
    [SerializeField] public int height = 20;
    private ulong[,] waveform;
    private ulong[,] rules = new ulong[8,4]{
        {0b01110010,0b10100110,0b01001110,0b10011010 },
        {0b10001101,0b01011001,0b10110001,0b01100101 }, 
        {0b10001101,0b10100110,0b01001110,0b01100101 }, 
        {0b10001101,0b01011001,0b01001110,0b10011010 }, 
        {0b01110010,0b01011001,0b10110001,0b10011010 }, 
        {0b01110010,0b10100110,0b10110001,0b01100101 },
        {0b10001101,0b10100110,0b10110001,0b10011010 },
        {0b01110010,0b01011001,0b01001110,0b01100101 }
    };
    private int[] _dx = new int[4] { 0, 1, 0, -1 };
    private int[] _dy = new int[4] { -1, 0, 1, 0 };


    public TileType GetTileAt(int x, int y)
    {
        ulong val = waveform[x, y];
        for (int cx = 7; cx > 0; cx--)
            if ((val & (1UL << cx)) != 0)
                return (TileType)cx;
        return TileType.Home;
    }

    private int typeCount = 0;
    private void Start()
    {
        typeCount = Enum.GetValues(typeof(TileType)).Length;
        TileType[] types = (TileType[])Enum.GetValues(typeof(TileType));

        
        waveform = new ulong[width, height];
        // At the beginning, everything is possible
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                waveform[x, y] = 0xFF;
            }
        }
        waveform[width / 2, height / 2] = 0x7F;
        waveform[width / 2+1, height / 2] = 0x7F;
        waveform[width / 2, height / 2-1] = 0x7F;
        waveform[width / 2+1, height / 2-1] = 0x7F;
        // No house at the player spawn please!

        // Maybe add some random stuff as seed
    }

    [SerializeField] private UnityEvent OnComplete;

    private bool completed = false;
    private void Update()
    {
        if (completed) return;

        for (int i = 0; i < 100000; i++)
        {
            if (SolveStep())
            {
                completed = true;
                OnComplete?.Invoke();
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
        waveform[x,y] = 1UL << (int)type;

        for (int dir = 0; dir < 4; dir++)
        {
            int nx = x + _dx[dir];
            int ny = y + _dy[dir];
            if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                continue;
            ulong allowed = rules[(int)type, dir];
            waveform[nx, ny] &= ~allowed;
        }
    }



    private (int x, int y)? FindWorstTile()
    {
        restart:
        List<(int x, int y, int count)> options = new List<(int, int, int)>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int count = GetPossibilityCount(x, y);
                if (count == 0)
                {
                    for (int cx = -5; cx <= 5; cx ++)
                        for (int cy = -5; cy <= 5; cy ++)
                        {
                            int nx = x + cx;
                            int ny = y + cy;
                            if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                                continue;
                            waveform[nx, ny] = 0xFF;
                        }

                    waveform[width / 2, height / 2] = 0x7F;
                    waveform[width / 2, height / 2 - 1] = 0x7F;
                    waveform[width / 2 + 1, height / 2] = 0x7F;
                    waveform[width / 2 + 1, height / 2 - 1] = 0x7F;

                    Debug.Log("Waveform collapse collision");
                    goto restart;
                }
                if (count > 1)
                    options.Add((x, y, count));
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
        ulong wave = waveform[x, y];
        int count = 0;
        for (int cx = 0; cx < 64; cx++)
            if ((wave & (1UL << cx)) != 0)
                count++;
        return count;
    }

    private TileType[] GetPossibilitiesAt(int x, int y)
    {
        List<TileType> types = new List<TileType>();

        for (int i = 0; i < typeCount; i++)
        {
            if ((waveform[x, y] & (1UL << i)) != 0)
                types.Add((TileType)i);
        }

        return types.ToArray();
    }
}

public enum TileType : byte
{
    Street13 = 0,
    Street02,
    Street01,
    Street03,
    Street23,
    Street12,
    Plaza,
    Home
}