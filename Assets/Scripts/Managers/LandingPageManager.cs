using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingPageManager : MonoBehaviour
{
    

    public string nextScene = "IntroScene";

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
