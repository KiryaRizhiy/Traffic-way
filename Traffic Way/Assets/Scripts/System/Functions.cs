using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Functions
{
    public static void DrawPolygonCollider(PolygonCollider2D collider)
    {
        if (collider.GetComponent<LineRenderer>() != null)
            MonoBehaviour.Destroy(collider.GetComponent<LineRenderer>());
        LineRenderer _lr = collider.gameObject.AddComponent<LineRenderer>();
        _lr.startWidth = 0.025f;
        _lr.endWidth = 0.025f;
        _lr.useWorldSpace = false;
        _lr.positionCount = collider.points.Length + 1;
        for (int i = 0; i < collider.points.Length; i++)
        {
            _lr.SetPosition(i,new Vector3(collider.points[i].x,collider.points[i].y));
        }
        _lr.SetPosition(collider.points.Length, new Vector3(collider.points[0].x, collider.points[0].y));
    }
    public static void ScalePolygonCollider(PolygonCollider2D collider,float scaleChange)
    {
        for (int i = 0; i < collider.points.Length; i++)
        {
            collider.points[i] = collider.points[i] * scaleChange;
        }
    }
    public static string EnumerableAsString(IEnumerable enumerable, char separator = '|')
    {
        string _s = "";
        foreach (System.Object _o in enumerable)
        {
            _s += separator + _o.ToString() + separator;
        }
        return _s;
    }
}