using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCarController : MonoBehaviour
{
    public bool crashed
    { get; private set; }
    private NPCCarDriver _drv;
    void Start()
    {
        crashed = false;
        _drv = transform.parent.GetComponent<NPCCarDriver>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "Car":
                //Logger.UpdateContent(UILogDataType.GameState, "Car crashed with " + gameObject.name);
                crashed = true;
                _drv.Crash();
                Engine.Events.CrashHappened();
                break;
            case "Bullet":
                //Logger.UpdateContent(UILogDataType.GameState, "Car crashed with " + gameObject.name);
                _drv.BulletHit();
                Destroy(collider.gameObject);
                break;
            default:
                Debug.Log("NPC car " + gameObject.name + "touched unknown object " + collider.name + ", tagged " + collider.tag);
                break;
        }
    }
}
