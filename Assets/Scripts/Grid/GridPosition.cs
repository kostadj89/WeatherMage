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
    public int floor;

    public GridPosition(int x, int y, int floor)
    {
        this.x = x;
        this.y = y;
        this.floor = floor;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
                x == position.x &&
                y == position.y && 
                floor == position.floor;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, floor);
    }

    public override string ToString()
    {
        return "(X:" + x + ", Y:" + y + ", Floor:" + floor + ")";
    }

    public static bool operator ==(GridPosition v, GridPosition u)
    {
        return (v.x == u.x) && (v.y == u.y) && (v.floor == u.floor);
    }

    public static bool operator !=(GridPosition v, GridPosition u)
    {
        return !(v == u);
    }

    public static GridPosition operator +(GridPosition v, GridPosition u)
    {
        return new GridPosition(v.x + u.x, v.y + u.y, v.floor + u.floor);
    }

    public static GridPosition operator -(GridPosition v, GridPosition u)
    {
        return new GridPosition(v.x - u.x, v.y - u.y, v.floor - u.floor);
    }
}
