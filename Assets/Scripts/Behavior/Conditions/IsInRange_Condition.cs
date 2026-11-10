using UnityEngine;

public class IsInRange_Condition : Behavior_Condition
{
    GameObject self;
    GameObject[] targets;
    float maxRange;

    public IsInRange_Condition(GameObject self, float maxRange, bool reverseCondition)
    {
        this.self = self;
        this.maxRange = maxRange;
        this.reverseCondition = reverseCondition;
    }
    public override bool Evaluate()
    {
        if (self == null) return false;
        targets = GameObject.FindGameObjectsWithTag("Player");
        bool result = false;
        foreach(var target in targets)
        {
            if (target.GetComponent<Player>().isDead)
            {
                continue;
            }
            if(!result)
            {
                result = CheckForReverseCondition(Vector3.Distance(self.transform.position, target.transform.position) <= maxRange);
            }
        }
        return result;
    }
}
