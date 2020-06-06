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
        Engine.Events.shieldDestroyed += OnShieldDestroyed;
        if (Settings.testMode) Functions.DrawPolygonCollider(GetComponent<PolygonCollider2D>());
    }

    private void OnShieldDestroyed()
    {
        transform.Translate(Vector2.down * Settings.carShieldDestroyRollBackDistance);
        StartCoroutine(Blink());
    }
    private void OnCrhashHappened()
    {
        currentSpeed = 0;
        crashed = true;
    }
    private void Accelerate()
    {
        if (currentSpeed < Engine.meta.car.speedLimit - Engine.meta.car.acceleration)
            currentSpeed += Engine.meta.car.acceleration;
        else
            currentSpeed = Engine.meta.car.speedLimit;
    }
    private void Break()
    {
        if (currentSpeed > Engine.meta.car.brakingSpeed)
            currentSpeed -= Engine.meta.car.brakingSpeed;
        else
            currentSpeed = 0;
    }

    void OnDestroy()
    {
        CurrentCar = null;
        Engine.Events.crashHappened -= OnCrhashHappened;
        Engine.Events.shieldDestroyed -= OnShieldDestroyed;
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
        Logger.UpdateContent(UILogDataType.Controls, "Car speed: " + currentSpeed);
    }

    private IEnumerator Blink()
    {
        SpriteRenderer _sr = GetComponent<SpriteRenderer>();
        float blinkPause = 0.06f;
        float alphaLevel = 0.15f;
        int blinks = 3;
        for (int i = 0; i < blinks; i++)
        {
            _sr.color = new Color(255,255,255,alphaLevel);
            yield return new WaitForSeconds(blinkPause);
            _sr.color = new Color(255, 255, 255, 1);
            yield return new WaitForSeconds(blinkPause);
        }
    }
}
