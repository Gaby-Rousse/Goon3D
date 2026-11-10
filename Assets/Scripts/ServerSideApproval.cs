using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
//Pour des raisons ben obscure c'est une invocation satanique avoir DEUX FOUTU PREFAB DIFFERENT
//MERCI BTW UNITY DE TOUJOURS **FUCKING** CHANGER LEUR LIEN MAIS NE FAIRE AUCUNE REDIRECTION, TROP COMPLIQUÉ AMIRIGHT
//WHO
//CARES?
//CERTAINEMENT PAS EUX.
//https://discussions.unity.com/t/what-is-the-best-way-to-spawn-a-different-avatar-for-each-connected-player/909307/4
//https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@1.15/manual/basics/connection-approval.html
public class ServerSideApproval : MonoBehaviour
{
    //le hash des prefabs, aller voir DefaultNetworkPrefabs
    uint kyoko = 1001;
    uint misako = 1002;
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        //NetworkManager.Singleton.StartHost(); ?
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        //C'est un jeux fait pour deux, mais jmenfou si vous êtes 1000.
        if(NetworkManager.Singleton.ConnectedClientsList.Count % 2 == 0)
        {
            response.PlayerPrefabHash = kyoko;
        }
        else
        {
            response.PlayerPrefabHash = misako;
        }

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

}