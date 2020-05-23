using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarShooter : MonoBehaviour
{
    private Coroutine shooting;
    private static GameObject bullet;

    public static void LoadResources()
    {
        bullet = Resources.Load<GameObject>("TrafficWay/Prefabs/PlayerCars/Accessories/Bullet");
    }

    void Start()
    {
        Engine.Events.crashHappened += StopShooting;
        Engine.Events.finishLineReached += StopShooting;
        shooting = StartCoroutine(Shooting());
    }
    void OnDestroy()
    {
        Engine.Events.crashHappened -= StopShooting;
        Engine.Events.finishLineReached -= StopShooting;
    }

    private void StopShooting()
    {
        StopCoroutine(shooting);
    }
    private IEnumerator Shooting()
    {
        while (true)
        {
            if (Engine.paused)
                yield return new WaitUntil(() => !Engine.paused);
            yield return new WaitForSeconds(Settings.shootFrequency);
            Instantiate(bullet, transform.position, Quaternion.identity);
        }
    }
}