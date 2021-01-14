using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriver : MonoBehaviour
{
    public static GameObject CurrentCar
    { get; private set; }
    public GameplayMode mode;

    private Engine.GameData.CarUpgradeData _engineData
    {
        get
        {
            return Engine.meta.garage.GetCarUpgrade(CarUpgradeType.Engine);
        }
    }
    private Engine.GameData.CarUpgradeData _brakesData
    {
        get
        {
            return Engine.meta.garage.GetCarUpgrade(CarUpgradeType.Brakes);
        }
    }
    private Engine.GameData.CarUpgradeData _gearboxData
    {
        get
        {
            return Engine.meta.garage.GetCarUpgrade(CarUpgradeType.Gearbox);
        }
    }
    private const float brakingCoeffitient = 1.7f; // 1.7f;
    private const float accelerationCoeffitient = 0.9f;// 0.9f;
    private const float fireMaxEnmission = 1800f;
    private ParticleSystem.EmissionModule _e;
    private ParticleSystem.MainModule _main;

    public static float currentSpeed
    {
        get;
        private set;
    }
    public float cameraMaxVelocityOffset;
    public float cameraConstantOffset;
    public bool accelerateAlways;
    private Transform shield 
    { get { return _nitroParticles.GetChild(2); } }
    private Transform _cam
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private Transform _celebrateParticles
    {
        get
        {
            return transform.GetChild(2).GetChild(0);
        }
    }
    private Transform _startParticles
    {
        get
        {
            return transform.GetChild(2).GetChild(1);
        }
    }
    private Transform _rearLeftWheelParticle
    {
        get
        {
            return _startParticles.GetChild(0);
        }
    }
    private Transform _rearRightWheelParticle
    {
        get
        {
            return _startParticles.GetChild(1);
        }
    }
    private Transform _stopParticles
    {
        get
        {
            return transform.GetChild(2).GetChild(2);
        }
    }
    private Transform _nitroParticles
    {
        get
        {
            return transform.GetChild(2).GetChild(3);
        }
    }
    private Transform _leftNitroFire
    {
        get
        {
            return _nitroParticles.GetChild(1);
        }
    }
    private Transform _rightNitroFire
    {
        get
        {
            return _nitroParticles.GetChild(0);
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
            ShowNitro();
            transform.localScale = Settings.carsScale;
            _celebrateParticles.localScale = new Vector3(1f / Settings.carsScale.x, 1f / Settings.carsScale.y, 1f / Settings.carsScale.z);
            _celebrateParticles.position += Vector3.up * cameraConstantOffset;
            currentSpeed = 0f;
            Engine.Events.crashHappened += OnCrhashHappened;
            Engine.Events.shieldDestroyed += OnShieldDestroyed;
            Engine.Events.nitroActivated += ShowNitro;
            if (Settings.testMode) Functions.DrawPolygonCollider(GetComponent<PolygonCollider2D>());
            _cam.gameObject.SetActive(true);
        }
        if (mode == GameplayMode.Puzzle)
        {
            shield.GetComponent<ParticleSystem>().Stop();
            _cam.gameObject.SetActive(false);
        }
    }

    private void ShowNitro()
    {
        //Engine.meta.car.isBoosted=true;                                                  //if true then car.shield.enabled=true
        if (Engine.meta.car.isBoosted == true)
        {
            shield.GetComponent<ParticleSystem>().Play();
            _leftNitroFire.GetComponent<ParticleSystem>().Play();
            _rightNitroFire.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            shield.GetComponent<ParticleSystem>().Stop();
        }
    }
    private void OnShieldDestroyed()
    {
        transform.Translate(Vector2.down * Settings.carShieldDestroyRollBackDistance);
        StartCoroutine(Blink());
        shield.GetComponent<ParticleSystem>().Stop();
    }
    private void OnCrhashHappened()
    {
        currentSpeed = 0;
        crashed = true;
    }
    private void Accelerate()
    {
        if (currentSpeed < Engine.meta.car.speedLimit * _gearboxData.upgradeRate - ComputeAcceleration(currentSpeed))
            currentSpeed += ComputeAcceleration(currentSpeed);
        else
            currentSpeed = Engine.meta.car.speedLimit * _gearboxData.upgradeRate;
    }
    private void Break()
    {
        if (currentSpeed > ComputeBraking(currentSpeed))
            currentSpeed -= ComputeBraking(currentSpeed);
        else
            currentSpeed = 0;
    }
    private float ComputeBraking(float currentSpeed)
    {
        float res = ((6f / Mathf.Log(currentSpeed + 10)) - 0.5f) * Engine.meta.car.brakingSpeed * _brakesData.upgradeRate;
        Logger.UpdateContent(UILogDataType.Controls, "Braking: " + res);
        return res;
    }
    private float ComputeAcceleration(float currentSpeed)
    {
        float res = ((6f / Mathf.Log(currentSpeed + 10)) - 0.5f) * Engine.meta.car.acceleration * _engineData.upgradeRate;
        Logger.UpdateContent(UILogDataType.Controls, "Acceleration: " + res);
        return res;
    }

    void OnDestroy()
    {
        CurrentCar = null;
        Engine.Events.crashHappened -= OnCrhashHappened;
        Engine.Events.shieldDestroyed -= OnShieldDestroyed;
        Engine.Events.nitroActivated -= ShowNitro;
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
        Logger.AddContent(UILogDataType.Controls, "Car speed: " + currentSpeed);
        if((ComputeBraking(currentSpeed) > brakingCoeffitient && !UserInteraction.gas && currentSpeed >0)
            || ( ComputeAcceleration(currentSpeed) > accelerationCoeffitient && UserInteraction.gas))
        {
            //draw lines
            LineRenderer _lr = new GameObject().AddComponent<LineRenderer>();
            _lr.material = new Material(Shader.Find("Sprites/Default"));
            _lr.SetPosition(0, _rearLeftWheelParticle.position + Vector3.forward * 8.5f);
            _lr.SetPosition(1, _rearLeftWheelParticle.position + Vector3.forward * 8.5f - Vector3.up * currentSpeed * Time.deltaTime);
            _lr.startColor = new Color(0.1981132f, 0.1981132f, 0.1981132f);
            _lr.endColor = new Color(0.1981132f, 0.1981132f, 0.1981132f);
            _lr.startWidth = 0.23f;
            _lr.endWidth = 0.23f;
            //Destroy(_lr.gameObject, 1.5f);


            _lr = new GameObject().AddComponent<LineRenderer>();
            _lr.material = new Material(Shader.Find("Sprites/Default"));
            _lr.SetPosition(0, _rearRightWheelParticle.position + Vector3.forward * 8.5f);
            _lr.SetPosition(1, _rearRightWheelParticle.position + Vector3.forward * 8.5f - Vector3.up * currentSpeed * Time.deltaTime);
            _lr.startColor = new Color(0.1981132f, 0.1981132f, 0.1981132f);
            _lr.endColor = new Color(0.1981132f, 0.1981132f, 0.1981132f);
            _lr.startWidth = 0.23f;
            _lr.endWidth = 0.23f;
            //Destroy(_lr.gameObject, 1.5f);
        }
        if(ComputeBraking(currentSpeed) > brakingCoeffitient && !UserInteraction.gas && currentSpeed > 0)
        {
            Transform _stopPrt = Instantiate(_stopParticles);
            _stopPrt.position = _stopParticles.position;
            _stopPrt.GetChild(0).GetComponent<ParticleSystem>().Play();
            _stopPrt.GetChild(1).GetComponent<ParticleSystem>().Play();
            Destroy(_stopPrt.gameObject, 5f);
        }
        if (ComputeAcceleration(currentSpeed) > accelerationCoeffitient && UserInteraction.gas)
        {
            Transform _startPrt = Instantiate(_startParticles);
            _startPrt.position = _startParticles.position;
            _main = _startPrt.GetChild(0).GetComponent<ParticleSystem>().main;
            _main.startSizeMultiplier = ComputeAcceleration(currentSpeed) / accelerationCoeffitient;
            _main.startSpeed = _main.startSpeedMultiplier * ComputeAcceleration(currentSpeed) / accelerationCoeffitient;
            _startPrt.GetChild(0).GetComponent<ParticleSystem>().Play();
            _main = _startPrt.GetChild(1).GetComponent<ParticleSystem>().main;
            _main.startSizeMultiplier = ComputeAcceleration(currentSpeed) / accelerationCoeffitient;
            _main.startSpeed = _main.startSpeedMultiplier * ComputeAcceleration(currentSpeed) / accelerationCoeffitient;
            _startPrt.GetChild(1).GetComponent<ParticleSystem>().Play();
            Destroy(_startPrt.gameObject, 5f);
        }
        _e = _leftNitroFire.GetComponent<ParticleSystem>().emission;
        _e.rateOverTimeMultiplier = fireMaxEnmission * currentSpeed / Engine.maxPossibleSpeed;
        _e = _rightNitroFire.GetComponent<ParticleSystem>().emission;
        _e.rateOverTimeMultiplier = fireMaxEnmission * currentSpeed / Engine.maxPossibleSpeed;
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