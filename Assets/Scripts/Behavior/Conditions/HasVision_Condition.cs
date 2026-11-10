using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HasVision_Condition : Behavior_Condition
{
    GameObject self;
    GameObject[] targets;
    float maxAngle;

    public HasVision_Condition(GameObject self, float maxAngle, bool reverseCondition)
    {
        this.self = self;

        this.maxAngle = maxAngle;
        this.reverseCondition = reverseCondition;
    }
    public override bool Evaluate()
    {
        bool result = false;
        targets = GameObject.FindGameObjectsWithTag("Player");
        foreach (var target in targets)
        {
            if(!result)
            {
                result = CheckForReverseCondition(true);

                Vector3 directionToTarget = target.transform.position - self.transform.position;

                directionToTarget.Normalize();

                // Validation de l'angle de vue
                float angleToTarget = Vector3.Angle(self.transform.forward, directionToTarget);
                if (angleToTarget > maxAngle / 2f)
                    result = CheckForReverseCondition(false);

                // Raycast pour savoir si l'AI peux voir la Target
                if (Physics.Raycast(self.transform.position, directionToTarget, out RaycastHit hit, float.MaxValue))
                {
                    // Si frappe un objet qui n'est pas la target
                    if (hit.collider.gameObject != target)
                        result = CheckForReverseCondition(false);
                }
            }
        }
        return result;
    }

}
