﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class NPCCarDriver : MonoBehaviour
{
    public int hitpoints;
    public bool destroyShield;
    public bool notStopOnCrash;
    public float moveSpeed;
    public float rotationSpeed;
    public CarMoveType moveType;
    public CircleMoveDiraction circleMoveDiraction;
    public bool lookAtNextWaypoint;
    public float initialAngle;
    public CarType carType;
    public WaypointsType waypointsType;
    public StartEventType startWhen;
    public GameObject zone;

    private GameObject car;
    private List<Transform> waypoints;
    private Coroutine moving;
    private bool crashed
    {
        get
        {
            if (car != null)
                return car.GetComponent<NPCCarController>().crashed;
            else
                return true;
        }
        set
        {
            if (car != null)
                car.GetComponent<NPCCarController>().crashed = value;
        }
    }
    private int hitpointsLeft;

    private static GameObject[] RegularCars;
    private static GameObject[] Bosses;
    private static GameObject[] Special;
    private static Texture2D WideCircle;
    private static Texture2D NormalCircle;
    private static Material LineMaterial;
    private const float CircleDiameter = 0.6f;
    public static void LoadResources()
    {
        RegularCars = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/NPCCars/Regular");
        Bosses = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/NPCCars/Bosses");
        Special = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/NPCCars/Special");
        WideCircle = Resources.Load<Texture2D>("TrafficWay/Textures/NPCCars/Waypoints/WideCircle");
        NormalCircle = Resources.Load<Texture2D>("TrafficWay/Textures/NPCCars/Waypoints/NormalCircle");
        LineMaterial = Resources.Load<Material>("TrafficWay/Textures/NPCCars/Waypoints/LineMaterial");
    }

    void Start()
    {
        waypoints = new List<Transform>();
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            waypoints.Add(transform.GetChild(0).GetChild(i));
        Validate();
        CreateCar(carType);
        //if (waypoints.Count > 0)
        //    car.transform.position = new Vector3(waypoints[0].position.x, waypoints[0].position.y, 0f);
        //if (waypoints.Count > 1 && lookAtNextWaypoint)
        //        car.transform.rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (waypoints[1].position - car.transform.position).normalized));
        //else
        //    car.transform.rotation = Quaternion.Euler(Vector3.forward * initialAngle);
        //if (moveType == CarMoveType.circle)
        //{
        //    car.transform.position = new Vector3(waypoints[1].position.x, waypoints[1].position.y, 0f);
        //    if (circleMoveDiraction == CircleMoveDiraction.clockwise)
        //        car.transform.rotation = Quaternion.Euler(Vector3.forward *
        //            (Vector3.SignedAngle(Vector3.up, (waypoints[1].position - waypoints[0].position), Vector3.forward) - 90));
        //    if (circleMoveDiraction == CircleMoveDiraction.counterclockwise)
        //        car.transform.rotation = Quaternion.Euler(Vector3.forward *
        //            (Vector3.SignedAngle(Vector3.up, (waypoints[1].position - waypoints[0].position), Vector3.forward) + 90));
        //}
        ShowTraces();
        switch (startWhen)
        {
            case StartEventType.zoneReached:
                Engine.Events.zoneReached += Go;
                break;
            case StartEventType.zoneLeft:
                Engine.Events.zoneLeft += Go;
                break;
            case StartEventType.instant:
                Go(null);
                break;
            default:
                Debug.LogError("Unknown start event type " + startWhen.ToString());
                crashed = true;
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "NPC car driver unknown start event type " + startWhen.ToString());
                break;
        }
        hitpointsLeft = hitpoints;
    }
    public void Crash()
    {
        StopCoroutine(moving);
    }
    public void BulletHit()
    {
        hitpointsLeft--;
        if (hitpointsLeft < 1)
        {
            StopAllCoroutines();
            Destroy(car);
        }
    }
    public void ShieldHit()
    {
        StopAllCoroutines();
        Destroy(car);
    }

    private void Validate()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError(gameObject.name + " has no attached waypoints. At least one waypoint required");
            crashed = true;
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "NPC car driver " + gameObject.name + " has no attached waypoints. At least one waypoint required");
        }
        if (startWhen != StartEventType.instant && zone == null)
        {
            Debug.LogError(gameObject.name + " has start event type, but no zone attached. Moving will not be started");
            crashed = true;
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "NPC car driver " + gameObject.name + " has start event type, but no zone attached. Moving will not be started");
        }
        if (moveType == CarMoveType.circle && waypoints.Count < 2)
        {
            Debug.LogError(gameObject.name + " cant move on circle trajectory having less than 2 waypoints");
            crashed = true;
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "NPC car driver " + gameObject.name + " cant move on circle trajectory having less than 2 waypoints");
        }
    }
    private void CreateCar(CarType type)
    {
        Vector3 _position;
        Quaternion _rotation;
        _position = waypoints[0].position;
        if (waypoints.Count > 1 && lookAtNextWaypoint)
            _rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (waypoints[1].position - waypoints[0].position).normalized));
        else
            _rotation = Quaternion.Euler(Vector3.forward * initialAngle);
        if (moveType == CarMoveType.circle)
        {
            _position = waypoints[1].position;
            if (circleMoveDiraction == CircleMoveDiraction.clockwise)
                _rotation = Quaternion.Euler(Vector3.forward *
                    (Vector3.SignedAngle(Vector3.up, (waypoints[1].position - waypoints[0].position), Vector3.forward) - 90));
            if (circleMoveDiraction == CircleMoveDiraction.counterclockwise)
                _rotation = Quaternion.Euler(Vector3.forward *
                    (Vector3.SignedAngle(Vector3.up, (waypoints[1].position - waypoints[0].position), Vector3.forward) + 90));
        }

        switch (type)
        {
            case CarType.regular:
                car = Instantiate(RegularCars[Random.Range(0, RegularCars.Length - 1)],_position,_rotation,transform);
                break;
            case CarType.boss:
                car = Instantiate(Bosses[Random.Range(0, Bosses.Length - 1)],_position,_rotation, transform);
                break;
            case CarType.excavator:
                foreach (GameObject _obj in Special)
                    if (_obj.name.Contains("Excavator"))
                        car = Instantiate(_obj,_position,_rotation, transform);
                break;
            case CarType.rink:
                foreach (GameObject _obj in Special)
                    if (_obj.name.Contains("Rink"))
                        car = Instantiate(_obj,_position,_rotation, transform);
                break;
            case CarType.truck:
                foreach (GameObject _obj in Special)
                    if (_obj.name.Contains("Truck"))
                        car = Instantiate(_obj,_position,_rotation, transform);
                break;
            case CarType.train:
                foreach (GameObject _obj in Special)
                    if (_obj.name.Contains("Train"))
                        car = Instantiate(_obj,_position,_rotation, transform);
                break;
            case CarType.police:
                foreach (GameObject _obj in Special)
                    if (_obj.name.Contains("Police"))
                        car = Instantiate(_obj,_position,_rotation, transform);
                break;
        }
        car.tag = Tags.NPCCar.ToString();
        car.AddComponent<NPCCarController>();
        car.transform.localScale = Settings.carsScale;
        if (Settings.testMode) Functions.DrawPolygonCollider(car.GetComponent<PolygonCollider2D>());
    }
    private void ShowTraces()
    {
        if (waypoints.Count == 0)
            return;
        waypoints[0].parent.position += Vector3.forward * 0.5f;
        if (waypointsType == WaypointsType.invisible)
            return;
        if (waypointsType == WaypointsType.normalCircles || waypointsType == WaypointsType.wideCircles)
        {
            Texture2D _tex = null;
            if (waypointsType == WaypointsType.normalCircles)
                _tex = NormalCircle;
            if (waypointsType == WaypointsType.wideCircles)
                _tex = WideCircle;
            if (waypoints.Count > 1)
            {
                LineRenderer _lr = waypoints[0].parent.gameObject.AddComponent<LineRenderer>();
                _lr.numCornerVertices = 3;
                _lr.useWorldSpace = false;
                _lr.textureMode = LineTextureMode.Tile;
                _lr.material = LineMaterial;
                _lr.startWidth = Settings.tracesWidth;
                _lr.endWidth = Settings.tracesWidth;
                _lr.positionCount = waypoints.Count;
                _lr.SetPosition(0, waypoints[0].localPosition + (waypoints[1].position - waypoints[0].position).normalized * CircleDiameter);
                for (int i = 1; i < waypoints.Count - 1; i++)
                {
                    Debug.Log("Draw position " + i);
                    _lr.SetPosition(i, waypoints[i].localPosition);
                }
                _lr.SetPosition(waypoints.Count - 1, waypoints[waypoints.Count - 1].localPosition - (waypoints[waypoints.Count - 1].position - waypoints[waypoints.Count - 2].position).normalized * CircleDiameter);
                //LineRenderer _lr;
                //for (int i = 0; i < waypoints.Count - 1; i++)
                //{
                //    waypoints[i].rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (waypoints[i + 1].position - waypoints[i].position).normalized));
                //    _lr = waypoints[i].gameObject.AddComponent<LineRenderer>();
                //    _lr.useWorldSpace = false;
                //    _lr.textureMode = LineTextureMode.Tile;
                //    _lr.material = LineMaterial;
                //    _lr.startWidth = Settings.tracesWidth;
                //    _lr.endWidth = Settings.tracesWidth;
                //    _lr.SetPosition(0, Vector3.zero);
                //    _lr.SetPosition(1, Vector3.up * (Vector2.Distance(waypoints[i].position, waypoints[i + 1].position)));
                //    waypoints[i].localScale = Vector3.one;
                //}
                waypoints[0].rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (waypoints[1].position - waypoints[0].position).normalized));                
                waypoints[0].gameObject.AddComponent<SpriteRenderer>();
                waypoints[0].GetComponent<SpriteRenderer>().sprite = Sprite.Create(_tex, new Rect(0.0f, 0.0f, _tex.width, _tex.height), new Vector2(0.5f, 0.5f));
                waypoints[0].GetComponent<SpriteRenderer>().color = Color.white;
                waypoints[0].localScale = Settings.carsScale;
                waypoints[waypoints.Count - 1].rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (waypoints[waypoints.Count - 2].position - waypoints[waypoints.Count - 1].position).normalized));
                waypoints[waypoints.Count - 1].gameObject.AddComponent<SpriteRenderer>();
                waypoints[waypoints.Count - 1].GetComponent<SpriteRenderer>().sprite = Sprite.Create(_tex, new Rect(0.0f, 0.0f, _tex.width, _tex.height), new Vector2(0.5f, 0.5f));
                waypoints[waypoints.Count - 1].GetComponent<SpriteRenderer>().color = Color.white;
                waypoints[waypoints.Count - 1].localScale = Settings.carsScale;
            }
            if (waypoints.Count == 1)
            {
                waypoints[0].gameObject.AddComponent<SpriteRenderer>();
                waypoints[0].GetComponent<SpriteRenderer>().sprite = Sprite.Create(_tex, new Rect(0.0f, 0.0f, _tex.width, _tex.height), new Vector2(0.5f, 0.5f));
                waypoints[0].GetComponent<SpriteRenderer>().color = Color.white;
                waypoints[0].localScale = Settings.carsScale;
            }
        }
    }
    private void Go(GameObject zoneSender)
    {
        if (startWhen != StartEventType.instant && zone != zoneSender)
            return;
        if (waypoints.Count > 0)
        {
            switch (moveType)
            {
                case CarMoveType.once:
                    car.transform.position = waypoints[0].position;
                    moving = StartCoroutine(OnceMove());
                    break;
                case CarMoveType.round:
                    car.transform.position = waypoints[0].position;
                    StartCoroutine(RoundMove());
                    break;
                case CarMoveType.repeat:
                    car.transform.position = waypoints[0].position;
                    StartCoroutine(RepeatMove());
                    break;
                case CarMoveType.cycled:
                    car.transform.position = waypoints[0].position;
                    StartCoroutine(CycledMove());
                    break;
                case CarMoveType.rotation:
                    car.transform.position = waypoints[0].position;
                    moving = StartCoroutine(RotateCar());
                    break;
                case CarMoveType.circle:
                    car.transform.position = waypoints[1].position;
                    moving = StartCoroutine(CircleMove());
                    break;
                default:
                    Debug.Log("Unknown move type " + moveType.ToString());
                    crashed = true;
                    break;
            }
        }
        car.transform.position -= Vector3.forward * 0.01f;
    }

    private IEnumerator OnceMove()
    {
        foreach (Transform _t in waypoints)
        {
            if (lookAtNextWaypoint && _t.position != car.transform.position)            
                car.transform.rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (_t.position - car.transform.position).normalized));            
            yield return StartCoroutine(GoToWaypoint(_t));
        }
    }
    private IEnumerator RoundMove()
    {
        while (true)
        {
            moving = StartCoroutine(OnceMove());
            yield return moving;
        }
    }
    private IEnumerator RepeatMove()
    {
        while (true)
        {
            //car.transform.position = waypoints[0].position;
            //car.transform.position -= Vector3.forward * 0.01f; 
            //if (waypoints.Count > 1 && lookAtNextWaypoint)
            //    car.transform.rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, (waypoints[1].position - car.transform.position).normalized));
            //else
            //    car.transform.rotation = Quaternion.Euler(Vector3.forward * initialAngle);
            moving = StartCoroutine(OnceMove());
            yield return moving;
            Destroy(car);
            CreateCar(carType);
        }
    }
    private IEnumerator CycledMove()
    {
        while (true)
        {
            moving = StartCoroutine(OnceMove());
            yield return moving;
            waypoints.Reverse();
        }
    }
    private IEnumerator GoToWaypoint(Transform waypoint)
    {
        Vector3 _absoluteDiraction = (waypoint.position - car.transform.position).normalized;
        Vector2 _relativeDiraction;
        //Turn Decart axis system
        //x = x cos a - y sin a
        //y = x sin a + y cos a
        //a = signed angle(vector2.up,car.transform.up)
        float _angle;
        _angle = Vector2.SignedAngle(new Vector2(car.transform.up.x, car.transform.up.y), Vector2.up) * Mathf.Deg2Rad;
        _relativeDiraction.x = _absoluteDiraction.x * Mathf.Cos(_angle) - _absoluteDiraction.y * Mathf.Sin(_angle);
        _relativeDiraction.y = _absoluteDiraction.x * Mathf.Sin(_angle) + _absoluteDiraction.y * Mathf.Cos(_angle);
        //Debug.Log("Going to waypoint " + waypoint.name + 
        //    " Waypoint info: Angle between " + car.transform.up + 
        //    " and " + Vector2.up + " is " 
        //    + _angle + " rad. Relative diraction is " + _relativeDiraction + 
        //    " absolute diraction is " + _absoluteDiraction);
        Vector2 _initialPosition = car.transform.position;
        while (Vector2.Distance(_initialPosition, car.transform.position) < Vector2.Distance(_initialPosition, waypoint.position))
        {
            if (Engine.paused)
                yield return new WaitUntil(() => !Engine.paused);
            if (crashed)
                yield return new WaitUntil(() => false);
            car.transform.Translate(_relativeDiraction * moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator RotateCar()
    {
        while (true)
        {
            if (Engine.paused)
                yield return new WaitUntil(() => !Engine.paused);
            if (crashed)
                yield return new WaitUntil(() => false);
            car.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator CircleMove()
    {
        Vector3 rotationAxis;
        if (circleMoveDiraction == CircleMoveDiraction.clockwise)
            rotationAxis = Vector3.back;
        else
            rotationAxis = Vector3.forward;
        while (true)
        {
            if (Engine.paused)
                yield return new WaitUntil(() => !Engine.paused);
            if (crashed)
                yield return new WaitUntil(() => false);
            car.transform.RotateAround(waypoints[0].position, rotationAxis, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
public enum CarMoveType {once,cycled,repeat,round,rotation,circle}
public enum CarType {regular,excavator,rink,truck,boss,train,police}
public enum WaypointsType {invisible, normalCircles, wideCircles}
public enum StartEventType { instant, zoneReached, zoneLeft }
public enum CircleMoveDiraction { clockwise, counterclockwise }