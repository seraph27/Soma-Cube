using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject hintIcon;
    public GameObject restartIcon;
    public GameObject menuIcon;
    public GameObject settingsCanvas;
    public Text settingsGUI;
    public Text menuGUI;
    PlayerManager playerController;

    private void Start() {
        playerController = GameObject.Find("Player").GetComponent<PlayerManager>();
        settingsCanvas.SetActive(false);
        hintIcon.SetActive(true);
        restartIcon.SetActive(true);
        menuIcon.SetActive(false);
    }

    public void Settings(){
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
        hintIcon.SetActive(!hintIcon.activeSelf);
        restartIcon.SetActive(!restartIcon.activeSelf);
        menuIcon.SetActive(!menuIcon.activeSelf);

        
        if(settingsCanvas.activeSelf){
            settingsGUI.text = "Resume";
            /*
            settingsGUI.color = new Color32(241, 248, 192, 233);
            menuGUI.color = new Color32(241, 248, 192, 233);
            */
        } else {
            settingsGUI.text = "Settings";
            /*
            settingsGUI.color = new Color32(56, 56, 56, 251);
            menuGUI.color = new Color32(56, 56, 56, 251);
            */
        }



    }

    public void UseHint(){
        playerController.IsInGrid();
    }

    public void GoToMenu(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Restart(){
        playerController.ResetAllPieces();
    }


    public void OpenEgroveURL()
    {
        Application.OpenURL("https://egrove.education/spatial-vis-daily-challenge");
    }
}
