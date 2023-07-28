using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//the structure which keeps data about grid tile location
public struct GridPosition : IEquatable<GridPosition>
{
    //y are rows, x are columns :3
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
                x == position.x &&
                y == position.y;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public override string ToString()
    {
        return "(X:" + x + ",Y:" + y + ")";
    }

    public static bool operator ==(GridPosition v, GridPosition u)
    {
        return (v.x == u.x) && (v.y == u.y);
    }

    public static bool operator !=(GridPosition v, GridPosition u)
    {
        return !(v == u);
    }

    public static GridPosition operator +(GridPosition v, GridPosition u)
    {
        return new GridPosition(v.x + u.x, v.y + u.y);
    }

    public static GridPosition operator -(GridPosition v, GridPosition u)
    {
        return new GridPosition(v.x - u.x, v.y - u.y);
    }
}
