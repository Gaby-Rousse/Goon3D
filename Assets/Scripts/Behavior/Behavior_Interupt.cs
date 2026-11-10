using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;

public class Behavior_Interupt
{
    // Behavior_Interupt est une classe qui se charge de relancer la recherche dans le BT lorsqu’une condition change d’état.
    // Elle peut contenir plusieurs conditions, et si l’une d’elles change, elle relancera le BT.
    // Par exemple : relancer l’arbre si l’IA a perdu de vue le joueur.

    Behavior_Tree behavior_Tree;
    Behavior_Condition[] behavior_Conditions;
    bool[] conditionState;

    public Behavior_Interupt(Behavior_Tree behavior_Tree, Behavior_Condition[] behavior_Conditions)
    {
        this.behavior_Tree = behavior_Tree;
        this.behavior_Conditions = behavior_Conditions;
        conditionState = new bool[behavior_Conditions.Length];

        UpdateState(); // Met à jour l’état des conditions au lancement de l’IA.
        CheckInterupt(); // Lance la fonction asynchrone qui restera active tant que l’IA sera en activité.
    }

    //POURQUOI TU ROULE TOUT LE TEMPS 
    //TU ARRETE JAMAIS
    //TU ROULE TU ROULE TU ROULE
    //LA GALAXIE POURRAIS EXPLOSER
    //MAIS TU VA ENCORE
    //... ROULER.
    
    async void CheckInterupt() // Fonction qui, toutes les 0,1 secondes, vérifie si une condition a changé de résultat. 
                               // Si c’est le cas, elle lance un Interupt au BT et met à jour l’état des conditions.
    {
        //meurt meurt steplait
        if (behavior_Tree == null || !behavior_Tree.enabled)
            return;
        try
        {
            for (int i = 0; i < behavior_Conditions.Length; i++)
            {
                if (behavior_Conditions[i].Evaluate() != conditionState[i])
                {
                    behavior_Tree.Interupt(); // Arrête l’action et relance le BT.
                    UpdateState();
                }
            }
        }
        //menfou awaye
        catch(System.Exception ignored)
        {
            return;
        }
        

        await Task.Delay(100);

        CheckInterupt(); // Fonction récursive/infinie.
    }

    void UpdateState() // Met à jour l’état des conditions.
    {
        for (int i = 0; i < behavior_Conditions.Length; i++)
        {
            conditionState[i] = behavior_Conditions[i].Evaluate();
        }
    }
}
