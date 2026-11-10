using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class Behavior_Composite : Behavior_Node
// Behavior_Composite est une sous-classe de Node qui représente un node servant à l’exécution d’actions via deux types de composites : les Selectors et les Sequences.
// Un Selector évalue ses nodes un par un jusqu’à en trouver un dont les conditions sont respectées. Il exécutera ensuite ce node (et pas les autres).
// Un Sequence évalue et exécute les nodes un par un jusqu’à ce qu’un node échoue son évaluation. Cela mettra fin à la Sequence.
// Il existe d’autres types de composites comme le "Parallel", mais ils ne sont pas couverts dans ce cours.
{
    public enum CompositeType { Selector, Sequence, Parallel }; // Déclaration d’un enum pour distinguer plus facilement les deux types.

    public CompositeType compositeType;
    Behavior_Node[] nodes;
    public Behavior_Tree behaviorTree;

    int sequenceIndex = 0;
    int selectorIndex = 0;

    public Behavior_Composite(CompositeType compositeType, Behavior_Node[] nodes, Behavior_Tree behaviorTree, Behavior_Condition[] conditions) : base(conditions)
    {
        this.compositeType = compositeType;
        this.nodes = nodes;
        this.behaviorTree = behaviorTree;
    }

    public override void ExecuteAction(Behavior_Composite parentComposite) // ExecuteAction aura ici un comportement différent selon le type de Composite.
    {
        base.ExecuteAction(parentComposite);
        switch (compositeType)
        {
            case CompositeType.Selector:
                SelectorStart(0);
                return;
            case CompositeType.Sequence:
                SequenceContinue(0); // Commence la Sequence à l’index 0.
                return;
            case CompositeType.Parallel:
                ParallelStart();
                return;
        }
    }

    override public void FinishAction(bool result)
    {
        if (compositeType == CompositeType.Sequence && sequenceIndex < nodes.Length - 1)
        // FinishAction, dans le cas d’une Sequence, ne signifie pas que le Composite est nécessairement terminé.
        // Cela veut dire qu’il doit passer au prochain node. Il sera terminé lorsqu’il aura exécuté tous ses nodes (ou échoué une évaluation ou action).
        {
            if (!result)
            {
                EndComposite(false);
                return;
            }
            SequenceContinue(sequenceIndex + 1);
            return;
        }
        else if(compositeType == CompositeType.Selector && !result)
        // FinishAction, dans le cas d’un Selector, ne signifie pas que le Composite est nécessairement terminé.
        // Si l'action à échouer, il devra passer au prochain node.
        {
            if (selectorIndex < nodes.Length - 1)
            {
                SelectorStart(selectorIndex + 1);
                return;
            }

            EndComposite(false);
            return;
        }

            EndComposite(true);
    }

    void EndComposite(bool result)
    {

        if (parentComposite != null)
            parentComposite.FinishAction(result);
        else
        {
            behaviorTree.RunTree(); // Si le Composite n’a pas de parent, c’est la racine. On relance donc le BT.
        }
            
    }

    // Selector
    // Le Selector parcourt les nodes jusqu’à en trouver un qui valide ses conditions.
    void SelectorStart(int startIndex)
    {
        for (int i = startIndex; i < nodes.Length; i++)
        {
            if (!nodes[i].EvaluateConditions())
                continue;
            else
            {
                selectorIndex = i;
                nodes[i].ExecuteAction(this);
                return;
            }
        }
        FinishAction(false); // N'a pas trouvé de Node qui fonctionne
    }

    // Sequence
    // La Sequence évalue et exécute les nodes un par un jusqu’à ce qu’un node échoue son évaluation. Cela met fin à la Sequence.
    void SequenceContinue(int index)
    {
        sequenceIndex = index;
        if (!nodes[sequenceIndex].EvaluateConditions())
        {
            FinishAction(false);
        }
        else
            nodes[sequenceIndex].ExecuteAction(this);
    }

    void ParallelStart()
    {
        foreach(var node in nodes)
        {
            if(!node.EvaluateConditions())
            {
                FinishAction(false);
                return;
            }
        }
        foreach (var node in nodes)
        {
            node.ExecuteAction(this);
        }
    }
}