using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectUiScript : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [SerializeField] private GameObject networkMenu;

    void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
    }

    private void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
        networkMenu.SetActive(false);
    }
    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
        networkMenu.SetActive(false);
    }
}
