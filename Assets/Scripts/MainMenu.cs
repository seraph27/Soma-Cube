using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject LevelGenerator;

    public void Start(){

    }

    public void SelectLevel(string difficulty) {
        Variable.difficulty = difficulty;
        Debug.Log("Select Level");
        SceneManager.LoadScene("SomaCube");
    }

    public void QuitGame() {
        Debug.Log("Quit");
        Application.Quit();
    }
}
