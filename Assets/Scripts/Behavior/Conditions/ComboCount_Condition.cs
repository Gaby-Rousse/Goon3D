using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class ComboCount_Condition : Behavior_Condition
{
    Enemy script;
    int currentCombo;

    public ComboCount_Condition(Enemy script, int currentCombo, bool reverseCondition)
    {
        this.script = script;
        this.currentCombo = currentCombo;
    }
    public override bool Evaluate()
    {
        return CheckForReverseCondition(script.ComboCount == currentCombo);
    }
}
