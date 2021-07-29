using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public GameObject imagesMode;
    public GameObject colourMode;

    public void Start()
    {
        if(Settings.isAbstract)
        {
            colourMode.SetActive(true);
            imagesMode.SetActive(false);
        }
        else
        {
            imagesMode.SetActive(true);
            colourMode.SetActive(false);
        }
    }
    public void OnClick() {
        SceneManager.LoadScene("GameScene");
    }

    public void Update()
    {
        if(Input.GetKeyDown("f9"))
        {
            Settings.isAbstract = !Settings.isAbstract;
            Start();
        }
    }
}
