using UnityEngine;

abstract public class Behavior_Condition
    // Classe abstraite qui servira de base pour toutes conditions. Chaque enfant devra implémenter Evaluate()
{
    protected bool reverseCondition;
    abstract public bool Evaluate();

    public bool CheckForReverseCondition(bool result)
    {
        if(reverseCondition)
            return !result;
        return result;

    }


}
