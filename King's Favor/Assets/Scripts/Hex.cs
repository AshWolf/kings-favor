using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public struct Hex
{
    public int q;
    public int r;

    public int s { get => -q - r; }

    private static Hex[] directions = {
        new(+1, 0), new(+1, -1), new(0, -1),
        new(-1, 0), new(-1, +1), new(0, +1),
    };

    public Hex(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public Hex(int q, int r, int s) : this(q, r)
    {
        Assert.AreEqual(this.s, s);
    }

    public static Hex Zero = new(0, 0);


    public override string ToString()
    {
        return $"({q}, {r}, {s})";
    }

    public Vector2 ToPixel(float size=0.5f)
    {
        var x = 3f/2 * q;
        var y = Mathf.Sqrt(3)/2 * q + Mathf.Sqrt(3) * r;
        return size * (new Vector2(x, y));
    }

    public static IEnumerable<Hex> Ring(int radius)
    {
        if(radius == 0)
        {
            yield return Hex.Zero;
            yield break;
        }

        Hex hex = directions[4] * radius;
        foreach (Hex dir in directions)
        {
            foreach (var _ in Enumerable.Range(0, radius))
            {
                yield return hex;
                hex += dir;
            }
        }
    }


    public static Hex operator +(Hex a, Hex b)
    {
        return new Hex(a.q + b.q, a.r + b.r);
    }

    public static Hex operator -(Hex a, Hex b)
    {
        return new Hex(a.q - b.q, a.r - b.r);
    }

    public static Hex operator -(Hex a)
    {
        return new Hex(-a.q, -a.r);
    }

    public static Hex operator *(int scalar, Hex hex)
    {
        return new Hex(scalar * hex.q, scalar * hex.r);
    }
    public static Hex operator *(Hex hex, int scalar)
    {
        return new Hex(scalar * hex.q, scalar * hex.r);
    }

}
