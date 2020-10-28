using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    private static GameObject _currentPuzzle;
    private static Transform _waypointsParent
    {
        get
        {
            return _currentPuzzle.transform.GetChild(0);
        }
    }
    private static int _puzzleFieldSide
    {
        get
        {
            return Mathf.RoundToInt(Mathf.Sqrt(_waypointsParent.childCount));
        }
    }
    private static float _averageWaypointDistance
    {
        get
        {
            return (
                Mathf.Abs(tiles[5, 0].tileTransform.position.x - tiles[0, 0].tileTransform.position.x) / 6 +
                Mathf.Abs(tiles[0, 5].tileTransform.position.y - tiles[0, 0].tileTransform.position.y) / 6 ) / 2;
        }
    }
    private static List<Transform> _waypointsList
    {
        get
        {
            List<Transform> _result = new List<Transform>();
            for(int i =0; i< _waypointsParent.childCount; i++)
            {
                _result.Add(_waypointsParent.GetChild(i));
            }
            return _result;
        }
    }
    private static TileData[,] tiles;
    private static List<PuzzleElementController> _elements;
    public static bool isAnyElementUnderControl
    {
        get
        {
            return _elements.Find(x => x._isUnderControl) != null;
        }
    }
    void Awake()
    {
        _currentPuzzle = gameObject;
        InitializeTiles();
        _elements = new List<PuzzleElementController>();
    }
    void OnDestroy()
    {
        _currentPuzzle = null;
    }
    private void InitializeTiles()
    {
        tiles = new TileData[_puzzleFieldSide, _puzzleFieldSide];
        List<Transform> _unmatchedWaypoints = new List<Transform>();
        _unmatchedWaypoints.AddRange(_waypointsList);
        float _minX, _minY;
        Transform _current;
        for (int y = 0; y < _puzzleFieldSide; y++)
        {
            _minY = float.PositiveInfinity;
            for (int x = 0; x < _puzzleFieldSide; x++)
            {
                _minX = float.PositiveInfinity;
                _current = _unmatchedWaypoints[0];
                foreach(Transform _t in _unmatchedWaypoints)
                    if(_t.position.x <= _minX && _t.position.y <= _minY)
                    {
                        _current = _t;
                        _minX = _t.position.x;
                        _minY = _t.position.y;
                    }
                _unmatchedWaypoints.Remove(_current);
                tiles[x, y] = new TileData(_current,x,y);
                //Debug.Log("[" + x + "," + y + "] element matched with " + _current.name + " " + _current.position);
            }
        }
    }
    public static Vector2 CheckInPuzzleElement(PuzzleElementController Element)
    {
        float _minDistance;
        foreach (TileData _t in tiles)
            if (_t.currentObject == Element.gameObject)
                _t.currentObject = null;
        Vector2 _displacement = Vector2.zero;
        foreach(Transform _elementWP in Element._waypointsList)
        {
            _minDistance = float.MaxValue;
            TileData _currentTile = tiles[0,0];
            foreach(TileData _t in tiles)
            {
                //Debug.Log("Checking " + _t.positionDesc);
                if (Vector3.Distance
                    (_elementWP.position - Vector3.forward * _elementWP.position.z
                    , _t.tileTransform.position - Vector3.forward * _t.tileTransform.position.z
                    ) < _minDistance)
                {
                    //Debug.Log("Tile " + _t.positionDesc + " selected for checking in " + Element.name + " " + _elementWP.name +
                    //    " instead of tile " + _currentTile.positionDesc +
                    //    " because current min distance " + _minDistance + " is less than distance between " + Element.name +
                    //    " position " + _elementWP.position + " and tile position " + _t.tileTransform.position +
                    //    "(" + Vector3.Distance
                    //    (_elementWP.position - Vector3.forward * _elementWP.position.z
                    //    , _t.tileTransform.position - Vector3.forward * _t.tileTransform.position.z) + ")") ;
                    _currentTile = _t;
                    _minDistance = Vector3.Distance
                        (_elementWP.position - Vector3.forward * _elementWP.position.z
                        , _t.tileTransform.position - Vector3.forward * _t.tileTransform.position.z);
                }
            }
            if (_currentTile.isBusy)
            {
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, Element.name + " checkin failure. Position [" + _currentTile.x + "," + _currentTile.y + "] is already busy with " + _currentTile.tileTransform.name);
                Debug.LogError(Element.name + " checkin failure on stage " + _elementWP.name +  ". Position [" + _currentTile.x + "," + _currentTile.y + "] is already busy with " + _currentTile.currentObject.name);
                _displacement = Vector2.zero;
                break;
            }
            else
            {
                //Debug.Log("Checking in " + Element.gameObject + " to tile " + _currentTile.positionDesc);
                _currentTile.currentObject = Element.gameObject;
                if(Vector2.Distance(_elementWP.position,_currentTile.tileTransform.position) > _displacement.magnitude)
                    _displacement = _currentTile.tileTransform.position - _elementWP.position;
            }
        }
        //Debug.Log(Element.name + " checked in with displacement " + _displacement);
        _elements.Add(Element);
        return _displacement;
    }
    public static Vector2 ProcessMoveRequest(PuzzleElementController Element, Vector2 Displacement)
    {
        //Если проекция Displacement на нормализованную ось элемента больше расстояния любой точки элемента до какого-либо свободного тайла,
        //То можем перемещать объект на расстояние от ближайшей точки элемента до этого тайла
        Vector2 _result = Vector2.zero;
        float _projection;
        _projection = (Displacement.x * Element.movingAxisVector.normalized.x + Displacement.y * Element.movingAxisVector.normalized.y); // Mathf.Sqrt(Mathf.Pow(Displacement.x, 2f) + Mathf.Pow(Displacement.y, 2f));
        //_projection *= Displacement.magnitude;
        if (Mathf.Abs(_projection) >= _averageWaypointDistance)
            _result = Element.movingAxisVector * _averageWaypointDistance * Mathf.Sign(_projection);
        Logger.UpdateContent(UILogDataType.Controls, "Move request with displacement " + Displacement + " prosessing."+
            " Projection on " + Element.movingAxisVector + " is " + _projection +
            ". Average distance is " + _averageWaypointDistance + ". ");
        int _xOffset = Mathf.RoundToInt(Mathf.Sign(_result.x) * Element.movingAxisVector.x);
        int _yOffset = Mathf.RoundToInt(Mathf.Sign(_result.y) * Element.movingAxisVector.y);
        foreach (TileData _t in tiles)
        {
            if (_t.currentObject == Element.gameObject)
            {
                if (
                    (_t.x + _xOffset) > (_puzzleFieldSide - 1)
                    ||
                    (_t.x + _xOffset) < 0
                    ||
                    (_t.y + _yOffset) > (_puzzleFieldSide - 1)
                    ||
                    (_t.y + _yOffset) < 0
                    )
                {
                    _result = Vector2.zero;
                    Debug.Log("Invalid move request. Index out of bounds. Tile position: [" + _t.x + "," + _t.y + "] "+
                        ", offset: [" + _xOffset + ", " + _yOffset + "] ");
                }
                else
                    if (tiles[_t.x + _xOffset, _t.y + _yOffset].isBusy && tiles[_t.x + _xOffset, _t.y + _yOffset].currentObject != Element.gameObject)
                {
                    _result = Vector2.zero;
                    Debug.Log("Invalid move request. Nearest tile busy with other object");
                }
            }
        }
        return _result;
    }
    private class TileData
    {
        public bool isBusy
        {
            get
            {
                return currentObject != null;
            }
        }
        public GameObject currentObject;
        public Transform tileTransform
        { get; private set; }
        public string positionDesc
        {
            get
            {
                return "[" + x + "," + y + "]";
            }
        }
        public  int x
        { get; private set; }
        public int y
        { get; private set; }
        public TileData(Transform TileTransform, int X, int Y)
        {
            tileTransform = TileTransform;
            x = X;
            y = Y;
        }
    }
}