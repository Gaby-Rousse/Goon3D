using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using UnityEngine;

abstract public class Behavior_Tree : NetworkBehaviour
{
    // Classe abstraite qui est la base de tout Behavior Tree. Elle implémente les fonctions de base présentes dans tout BT.

    protected Behavior_Composite root; // La racine d'un Behavior Tree sera toujours un Composite. Souvent un Selector, mais pas uniquement. Cette variable devra être créée dans chaque enfant de BT.
    public Behavior_Node activeNode; // L’activeNode représente l’action que le BT est actuellement en train d’exécuter. Il sert à lancer les Tick sur les Actions.
    public List<Behavior_Node> activeNodes = new();
    abstract public void InitializeTree(); // InitializeTree est la fonction qui permet de créer le BT. Elle contient la logique de l’arbre entier, incluant les Interrupts.

    private void Update()
    {
        if (root == null) return;
        if(!IsServer) return;
        //https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
        //In general, .NET collections do not support being enumerated and modified at the same time. If you try to modify the collection list during enumeration, it raises an exception.

        if (activeNodes.Count > 0)
        {
            List<Behavior_Node> clone = new List<Behavior_Node>(activeNodes);
            foreach (Behavior_Node node in clone)
            {
                //Si il est fini, éxécute le pas (ça drôlement ça plante pas)
                if (activeNodes.Contains(node))
                {
                    node.Tick(Time.deltaTime);
                }
            }
        }    
        else if (activeNode != null)
            activeNode.Tick(Time.deltaTime); // Réalise le Tick de l’action en cours.
    }
    public void RunTree()
    {
        root.ExecuteAction(null); // RunTree doit toujours simplement lancer l’exécution de sa racine. La racine se chargera de parcourir les nodes enfants.
    }

    private void Start()
    {
        if (!IsServer)
        {
            return;
        }
        InitializeTree(); // Création de l’arbre.
        RunTree(); // Exécution de l’arbre pour la première fois.
    }

    public void Interupt() // Arrête le node actuellement actif et relance le tree depuis le départ.
    {
        activeNode.Interupt();
        RunTree();
    }

    public void StopTree()
    {
        //retire le "node" actif
        if (activeNode != null)
        {
            activeNode = null;
        }

        //retire la liste de node actif (implémentation mid parallèle)
        activeNodes.Clear();

        //pu de root
        root = null;

        //bleh
        enabled = false;
    }
}
