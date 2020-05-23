using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriver : MonoBehaviour
{
    public static GameObject CurrentCar
    { get; private set; }
    public static float currentSpeed
    {
        get;
        private set;
    }

    private bool crashed = false;

    void Awake()
    {
        CurrentCar = gameObject;
    }

    void Start()
    {
        transform.localScale = Settings.carsScale;
        currentSpeed = 0f;
        Engine.Events.crashHappened += OnCrhashHappened;
        if (Settings.testMode) Functions.DrawPolygonCollider(GetComponent<PolygonCollider2D>());
    }

    private void OnCrhashHappened()
    {
        currentSpeed = 0;
        crashed = true;
    }
    private void Accelerate()
    {
        if (currentSpeed < Settings.carSpeedLimit - Settings.carAcceleration)
            currentSpeed += Settings.carAcceleration;
        else
            currentSpeed = Settings.carSpeedLimit;
    }
    private void Break()
    {
        if (currentSpeed > Settings.carBraking)
            currentSpeed -= Settings.carBraking;
        else
            currentSpeed = 0;
    }

    void OnDestroy()
    {
        CurrentCar = null;
        Engine.Events.crashHappened -= OnCrhashHappened;
    }

    // Update is called once per frame
    void Update()
    {
        if (Engine.paused || crashed)
            return;
        if (UserInteraction.gas)
            Accelerate();
        else
            Break();
        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
    }
}
