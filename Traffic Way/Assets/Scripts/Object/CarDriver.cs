using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriver : MonoBehaviour
{
    //сделать по аналогии load resources в carshooter
    public static GameObject CurrentCar
    { get; private set; }
    public GameplayMode mode;
    public static float currentSpeed
    {
        get;
        private set;
    }
    public float cameraMaxVelocityOffset;
    public float cameraConstantOffset;
    public bool accelerateAlways;
    private GameObject shield 
    { get { return transform.GetChild(3).gameObject; } }
    private Transform _cam
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private Transform _particles
    {
        get
        {
            return transform.GetChild(2);
        }
    }

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
        if (mode == GameplayMode.Drive)
        {
            //Engine.meta.car.isBoosted=true;                                                  //if true then car.shield.enabled=true
            if (Engine.meta.car.isBoosted == true)
            {
                shield.SetActive(true);
            }
            else
            {
                shield.SetActive(false);
            }
            transform.localScale = Settings.carsScale;
            _particles.localScale = new Vector3(1f / Settings.carsScale.x, 1f / Settings.carsScale.y, 1f / Settings.carsScale.z);
            _particles.position += Vector3.up * cameraConstantOffset;
            currentSpeed = 0f;
            Engine.Events.crashHappened += OnCrhashHappened;
            Engine.Events.shieldDestroyed += OnShieldDestroyed;
            if (Settings.testMode) Functions.DrawPolygonCollider(GetComponent<PolygonCollider2D>());
            _cam.gameObject.SetActive(true);
        }
        if (mode == GameplayMode.Puzzle)
        {
            shield.SetActive(false);
            _cam.gameObject.SetActive(false);
        }
    }

    private void OnShieldDestroyed()
    {
        transform.Translate(Vector2.down * Settings.carShieldDestroyRollBackDistance);
        StartCoroutine(Blink());
        shield.SetActive(false); 
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
        if (Engine.paused || crashed || mode == GameplayMode.Puzzle)
            return;
        if (UserInteraction.gas || (accelerateAlways && Settings.testMode))
            Accelerate();
        else
            Break();
        //transform.GetChild(0).GetComponent<Camera>().orthographicSize = 15 + (1.5f / (Settings.carSpeedLimit * 1.2f)) * currentSpeed;
        _cam.localPosition = new Vector3(0, cameraConstantOffset + (cameraMaxVelocityOffset / (Settings.carSpeedLimit * 1.2f)) * currentSpeed, _cam.localPosition.z);
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
public enum GameplayMode { Drive, Puzzle}
