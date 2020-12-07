using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public TMPro.TMP_Text playerNameTMP;
    public TMPro.TMP_InputField inputField;

    private void Start()
    {
        playerNameTMP.text = PlayerPrefs.GetString("PlayerName", "nobody");
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Basics");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SaveName()
    {
        PlayerPrefs.SetString("PlayerName", inputField.text);
        playerNameTMP.text = inputField.text;
    }
}
