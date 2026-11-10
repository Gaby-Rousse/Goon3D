using UnityEngine;
using static Behavior_Composite;

public class Behavior_Node
// Classe de base qui sera le parent de tout node (Action, Composite).
// Elle est responsable des éléments communs aux nodes, tels que la gestion des conditions, l’exécution d’un node et d’autres comportements de base.
{
    protected Behavior_Composite parentComposite;
    protected Behavior_Condition[] conditions;
    protected bool interupted = false;

    public Behavior_Node(Behavior_Condition[] conditions) // Le seul élément commun à tous les nodes est la possibilité d’avoir des conditions d’exécution.
    {
        this.conditions = conditions;
    }

    public bool EvaluateConditions() // EvaluateConditions n’est pas Virtual, car il sera toujours identique peu importe le type de node.
                                     // Son rôle est de parcourir la liste des conditions et de retourner True si elles sont respectées.
    {
        if (conditions == null)
            return true;

        foreach (Behavior_Condition condition in conditions)
            if (!condition.Evaluate())
                return false;

        return true;
    }

    virtual public void ExecuteAction(Behavior_Composite parentComposite) // ExecuteAction devra être Override dans les enfants.
                                                                          // La base est de réinitialiser l’interruption, de s’associer à son parent composite (si ce n’est pas la racine)
                                                                          // et de définir ce node comme actif.
    {
        interupted = false;
        this.parentComposite = parentComposite;
        SetNodeAsActive();
    }

    virtual public void FinishAction(bool result) // FinishAction doit être appelée uniquement si le node n’a pas été interrompu.
    {
        if (!interupted)
            parentComposite.FinishAction(result);
    }

    virtual public void Interupt() // Marque le node comme interrompu et appelle FinishAction.
    {
        interupted = true;
        FinishAction(false);
    }

    virtual public void Tick(float deltaTime) // Tick sera Override dans chaque Action et appelé par le BT, comme un Update() dans un MonoBehaviour.
    {
    }

    void SetNodeAsActive() // Met à jour le BT avec le node actif uniquement si la fonction est appelée par une Action (pas par un Composite).
    {
        if (parentComposite == null || this is Behavior_Composite)
            return;
        if(parentComposite.compositeType == CompositeType.Parallel)
        {
            parentComposite.behaviorTree.activeNodes.Add(this);
        }
        else
        {
            parentComposite.behaviorTree.activeNodes.Clear();
            parentComposite.behaviorTree.activeNode = this;
        }
    }
}