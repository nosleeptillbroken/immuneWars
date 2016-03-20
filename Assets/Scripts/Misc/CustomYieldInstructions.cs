using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Text;

public class WaitForSecondsOrUntil : CustomYieldInstruction
{
    float endTime = 0.0f;

    System.Func<bool> condition;

    public override bool keepWaiting
    {
        get
        {
            return (condition.Invoke() == false) && (Time.time < endTime);
        }
    }

    public WaitForSecondsOrUntil(float time, System.Func<bool> predicate)
    {
        endTime = Time.time + time;
        condition = predicate;
    }
}
