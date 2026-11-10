using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField IPinputField;
    string ip = "127.0.0.1";    // Ip par default est un roundabout. Donc se pointer soi-même 

    public void HostGame()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", 7777); // Le Host peux inscrire 0.0.0.0 pour accepté n'importe quelle IP pour la découverte (Auto find IP). Le port 7777 est le default de Unity
        NetworkManager.Singleton.StartHost(); // lancement du serveur en tant que Client (Listen Server)
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single); // Le changement de scène est différent pour le multijoueur, car le serveur doit changer de scène, pas juste le joueur
    }

    public void JoinGame()
    {
        if (IPinputField != null)
            ip = IPinputField.text; // Lire l'address Ip dans le text field du menu

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, 7777); // Connection via l'address lue dans le text field. Le port doit être le même que le Host
        NetworkManager.Singleton.StartClient(); // Démare la recherche du serveur. Pourrais être NULL et "freeze". Il serait judicieux de gérer les cas d'échec. (NetworkManager.Singleton.OnConnectionEvent, etc)


    }
}

