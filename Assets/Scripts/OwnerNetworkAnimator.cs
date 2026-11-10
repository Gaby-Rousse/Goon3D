using Unity.Netcode.Components;
using UnityEngine;

//ok. c'est pu drôle
//pourquoi... c'est pas un serializeField de NetworkAnimator
//C'EST LA SOLUTION OFFICIEL DE UNITY
//HEIN????????????????????????????????????????????????????????????????????????????????????????????????????????????
//Allez voir par vous même, on jase là:
//https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@1.13/manual/components/networkanimator.html#owner-authoritative-mode
public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

//Pourquoi OwnerNetworkAnimator?
//Je sais pas si c'est la bonne solution, MAIS. Les mouvements, ils proviennent du joueur, de l'owner, pas du serveur. 
//Donc, le client lui semble "voler" en boucle. Il fait un jump loop interminable. Selon le serveur, le client n'est pas grounded.
//Selon le client oui, il est grounded. Ici dorénavant, l'animation reflète le ressenti du client et non le ressenti du serveur.
//En ce qui concerne la sécurité. . .
//Pour l'invocation de la hitbox, dorénavant c'est seulement le serveur qui peut l'invoquer, donc les mouvement sont client
//et les invocations de membres sont serveur.