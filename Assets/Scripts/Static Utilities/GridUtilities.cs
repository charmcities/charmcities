using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtilities
{
    /// <summary>
    /// Rounds a position to the nearest square on the eight meter grid.
    /// </summary>
    /// <param name="start">The location before rounding</param>
    /// <param name="y">An optional new y value to replace the value in start.</param>
    /// <returns>The new location after rounding</returns>
    public static Vector3 RoundToNearestSquare(Vector3 start, float? y = null)
    {
        Vector3 end;
        end.x = RoundToNearestEight(start.x);
        if (y == null)
        {
            end.y = start.y;
        }
        else
        {
            end.y = (float)y;
        }
        end.z = RoundToNearestEight(start.z);
        return end;
    }

    /// <summary>
    /// Rounds a number to the nearest multiple of eight.
    /// </summary>
    /// <param name="n">Starting number</param>
    /// <returns>A value rounded to the nearest eight</returns>
    public static int RoundToNearestEight(float n)
    {
        return (int)System.Math.Round(n / 8, System.MidpointRounding.AwayFromZero) * 8;
    }

    /// <summary>
    /// Rounds a number to the nearest multiple of eight.
    /// </summary>
    /// <param name="n">Starting number</param>
    /// <returns>A value rounded to the nearest eight</returns>
    public static int RoundToNearestEight(int n)
    {
        return RoundToNearestEight((float)n);
    }
}
