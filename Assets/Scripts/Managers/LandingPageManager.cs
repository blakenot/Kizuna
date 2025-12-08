using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingPageManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
