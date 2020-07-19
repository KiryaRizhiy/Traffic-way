using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public bool trueFinishLine;
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.Car.ToString())
            if (trueFinishLine)
                Engine.Events.FinishLineReached();
            else
                Engine.Events.PassLineReached();
    }
}