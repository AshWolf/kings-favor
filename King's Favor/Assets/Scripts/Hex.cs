using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct Hex : IEquatable<Hex>
{
    public int q;
    public int r;

    [SerializeField]
    public int s { get => -q - r; }

    public readonly static Hex[] Directions = {
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
        return $"({q}, {r})";
    }

    public int Magnitude()
    {
        return (Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2;
    }

    public static int Distance(Hex a, Hex b)
    {
        return (a - b).Magnitude();
    }

    public Vector2 ToPixel(float size = 0.5f)
    {
        var x = 3f / 2 * q;
        var y = Mathf.Sqrt(3) / 2 * q + Mathf.Sqrt(3) * r;
        return size * (new Vector2(x, y));
    }

    public static Hex PixelToHex(Vector2 pixel, float size = 0.5f) {
        var q = (2f / 3 * pixel.x) / size;
        var r = (-1f / 3 * pixel.x + Mathf.Sqrt(3) / 3 * pixel.y) / size;

        return RoundHex(q, r);
    }

    private static Hex RoundHex(float qFloat, float rFloat)
    {
        var sFloat = -qFloat - rFloat;

        int q = Mathf.RoundToInt(qFloat);
        int r = Mathf.RoundToInt(rFloat);
        int s = Mathf.RoundToInt(sFloat);

        var q_diff = Math.Abs(q - qFloat);
        var r_diff = Math.Abs(r - rFloat);
        var s_diff = Math.Abs(s - sFloat);

        if (q_diff > r_diff && q_diff > s_diff)
        {
            q = -r - s;
        }
        else if (r_diff > s_diff)
        {
            r = -q - s;
        }
        else
        {
            s = -q - r;
        }

        return new Hex(q, r, s);
    }

    public static IEnumerable<Hex> Ring(int radius)
    {
        if (radius == 0)
        {
            yield return Hex.Zero;
            yield break;
        }

        Hex hex = Directions[4] * radius;
        foreach (Hex dir in Directions)
        {
            foreach (var _ in Enumerable.Range(0, radius))
            {
                yield return hex;
                hex += dir;
            }
        }
    }

    public override bool Equals(object other) {
        if (other is not Hex)
        {
            return false;
        }

        return Equals((Hex) other);
    }

    public bool Equals(Hex other) => q == other.q && r == other.r;

    public override int GetHashCode() => (q, r).GetHashCode();

    public static bool operator ==(Hex a, Hex b) => a.Equals(b);

    public static bool operator !=(Hex a, Hex b) => !(a == b);

    public static Hex operator +(Hex a, Hex b) => new(a.q + b.q, a.r + b.r);

    public static Hex operator -(Hex a, Hex b) => new(a.q - b.q, a.r - b.r);

    public static Hex operator -(Hex a) => new Hex(-a.q, -a.r);

    public static Hex operator *(int scalar, Hex hex) => new(scalar * hex.q, scalar * hex.r);

    public static Hex operator *(Hex hex, int scalar) => new(scalar * hex.q, scalar * hex.r);
}
