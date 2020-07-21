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
    public float cameraMaxVelocityOffset;
    public float cameraConstantOffset;

    private bool crashed = false;

    void Awake()
    {
        CurrentCar = gameObject;
        if (Engine.initialized)
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(
                Engine.CarsAppearences[Engine.meta.car.currentAppearenceNum],
                new Rect(0f, 0f, Engine.CarsAppearences[Engine.meta.car.currentAppearenceNum].width, Engine.CarsAppearences[Engine.meta.car.currentAppearenceNum].height),
                Vector2.one * 0.5f);
    }

    void Start()
    {
        transform.localScale = Settings.carsScale;
        transform.GetChild(1).localScale = new Vector3(1f / Settings.carsScale.x, 1f / Settings.carsScale.y, 1f / Settings.carsScale.z);
        transform.GetChild(1).position += Vector3.up * cameraConstantOffset;
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
        //transform.GetChild(0).GetComponent<Camera>().orthographicSize = 15 + (1.5f / (Settings.carSpeedLimit * 1.2f)) * currentSpeed;
        transform.GetChild(0).localPosition = new Vector3(0, cameraConstantOffset + (cameraMaxVelocityOffset / (Settings.carSpeedLimit * 1.2f)) * currentSpeed, transform.GetChild(0).localPosition.z);
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
