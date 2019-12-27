using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private Button findOpponentButton = null;

    private void Start() => SetUpInputField();
    private const string dupa = "PlayerName";


    private void SetUpInputField()
    {
        if (PlayerPrefs.HasKey(dupa))
        {
            findOpponentButton.interactable = false;
            return;
        }
        findOpponentButton.interactable = false;
        string defaultName = PlayerPrefs.GetString(dupa);
        nameInputField.text = defaultName;
        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }
    public void SavePlayerName()
    {
        string playerName = nameInputField.text;

        PhotonNetwork.NickName = playerName;

        PlayerPrefs.SetString(dupa, playerName);

        findOpponentButton.interactable = true;
    }
}
