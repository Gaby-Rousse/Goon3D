using System;
using System.Collections;
using UnityEngine;
public class Seconds_Condition : Behavior_Condition
{
    float seconds;
    float initialTime;

    public Seconds_Condition(float seconds, bool reverseCondition)
    {
        this.seconds = seconds;
        this.reverseCondition = reverseCondition;
        initialTime = Time.time;
    }
    public override bool Evaluate()
    {
        bool condition = CheckForReverseCondition(Time.time > seconds + initialTime);
        if(condition)
        {
           initialTime = Time.time;
        }

        return condition;
    }
}