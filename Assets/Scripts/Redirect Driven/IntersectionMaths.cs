using System;
using UnityEngine;

public static class IntersectionMaths
{
    public static bool IsIntersecting(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        var orientationOne = Orientation(p1, q1, p2);
        var orientationTwo = Orientation(p1, q1, q2);
        var orientationThree = Orientation(p2, q2, p1);
        var orientationFour = Orientation(p2, q2, q1);

        if (orientationOne != orientationTwo && orientationThree != orientationFour) return true;

        if (orientationOne == 0 && OnSegment(p1, p2, q1)) return true;

        if (orientationTwo == 0 && OnSegment(p1, q2, q1)) return true;

        if (orientationThree == 0 && OnSegment(p2, p1, q2)) return true;

        if (orientationFour == 0 && OnSegment(p2, q1, q2)) return true;

        return false;
    }

    private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        return q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) && 
               q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y);
    }

    private static int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        var value = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        if (value == 0) return 0; // colinear
        
        return value > 0 ? 1 : 2; // counter clockwise 
    }

    public static bool PointLiesOnSegment (Vector2 p, Vector2 a, Vector2 b, float t = 1E-03f)
    {
        // ensure points are collinear
        var zero = (b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y);
        if (zero > t || zero < -t) return false;
    
        // check if x-coordinates are not equal
        if (a.x - b.x > t || b.x - a.x > t)
            // ensure x is between a.x & b.x (use tolerance)
            return a.x > b.x
                ? p.x + t > b.x && p.x - t < a.x
                : p.x + t > a.x && p.x - t < b.x;
    
        // ensure y is between a.y & b.y (use tolerance)
        return a.y > b.y
            ? p.y + t > b.y && p.y - t < a.y
            : p.y + t > a.y && p.y - t < b.y;
    }
    
    public static Vector2? IntersectionPoint(Vector3 A, Vector2 B, Vector3 C, Vector2 D)
    {
        var a1 = B.y - A.y;
        var b1 = A.x - B.x;
        var c1 = a1 * A.x + b1 * A.y;

        var a2 = D.y - C.y;
        var b2 = C.x - D.x;
        var c2 = a2 * C.x + b2 * C.y;

        var determinant = a1 * b2 - a2 * b1;

        if (determinant == 0) return null;

        var x = (b2 * c1 - b1 * c2) / determinant;
        var y = (a1 * c2 - a2 * c1) / determinant;

        return new Vector2(x, y);
    }
    
    public static Vector2? FindIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        var dxSegOne = p2.x - p1.x;
        var dySegOne = p2.y - p1.y;
        var dxSegTwo = p4.x - p3.x;
        var dySegTwo = p4.y - p3.y;

        var denominator = dySegOne * dxSegTwo - dxSegOne * dySegTwo;
        
        var t1 = ((p1.x - p3.x) * dySegTwo + (p3.y - p1.y) * dxSegTwo) / denominator;

        if (float.IsInfinity(t1))
        {
            return null;
        }
        
        var intersection = new Vector2(p1.x + dxSegOne * t1, p1.y + dySegOne * t1);

        return intersection;
    }
}