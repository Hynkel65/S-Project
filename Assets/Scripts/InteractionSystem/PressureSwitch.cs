using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : Switch
{
    int numberColliding = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        numberColliding++;
        TurnOn();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        numberColliding--;
        if (numberColliding == 0)
            TurnOff();
    }
}
