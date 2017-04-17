using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFinishManager : CommonManager
{

    // Use this for initialization
    void Start()
    {
        MessageBox("t");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2)
        {
            BackFromGame = true;
            SceneManager.LoadScene("MainMenu");
            return;
        }
    }
}
