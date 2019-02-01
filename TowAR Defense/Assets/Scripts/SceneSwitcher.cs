using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public SceneField scene;

    public void SwitchScene()
    {
        SceneManager.LoadScene(scene);
    }
}
