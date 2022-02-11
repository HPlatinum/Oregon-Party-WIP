using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void LoadScene(int scene) {
        SceneManager.LoadScene(scene);
    }

    public string GetSceneName() {
        return(SceneManager.GetActiveScene().name);
    }
}
