using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public GameObject shapeMode;
    public GameObject colourMode;

    public void Start()
    {
        if(Settings.isShapes)
        {
            colourMode.SetActive(false);
            shapeMode.SetActive(true);
        }
        else
        {
            shapeMode.SetActive(false);
            colourMode.SetActive(true);
        }
    }
    public void OnClick() {
        SceneManager.LoadScene("GameScene");
    }

    public void Update()
    {
        if(Input.GetKeyDown("f9"))
        {
            Settings.isShapes = !Settings.isShapes;
            Start();
        }
    }
}
