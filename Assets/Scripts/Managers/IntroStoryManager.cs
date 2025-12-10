using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroStoryManager : MonoBehaviour
{
    [TextArea(5, 15)]
    public string[] storyLines;
    public TMP_Text storyText;
    public string nextScene = "MainStoryIntro";
    public float fadeTime = 2f;
    public float holdTime = 3f;

    private int index = 0;
    private bool done = false;
    private bool skipRequested = false;

    void Start()
    {
        storyText.text = "";
        StartCoroutine(PlayStory());
    }

    IEnumerator PlayStory()
    {
        
        while (index < storyLines.Length)
        {
            skipRequested = false;
            yield return StartCoroutine(FadeIn(storyLines[index]));

            float timer = 0f;
            while (timer < holdTime && !skipRequested)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            skipRequested = false;
            yield return StartCoroutine(FadeOut());
            index++;
        }

        
        done = true;

        
        yield return StartCoroutine(ShowFinalMessage("Tap to Begin"));
    }

    IEnumerator FadeIn(string text)
    {
        storyText.text = text;
        storyText.alpha = 0f;

        float t = 0f;
        while (t < fadeTime && !skipRequested)
        {
            t += Time.deltaTime;
            storyText.alpha = t / fadeTime;
            yield return null;
        }

        storyText.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        float t = fadeTime;

        while (t > 0f && !skipRequested)
        {
            t -= Time.deltaTime;
            storyText.alpha = t / fadeTime;
            yield return null;
        }

        storyText.alpha = 0f;
    }

    IEnumerator ShowFinalMessage(string message)
    {
        storyText.text = message;
        storyText.alpha = 0f;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            storyText.alpha = t / fadeTime;
            yield return null;
        }

        storyText.alpha = 1f;
    }

    void Update()
    {
        bool tapped =
            Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (!tapped) return;

        if (!done)
        {
          
            skipRequested = true;
        }
        else
        {
            
            SceneManager.LoadScene(nextScene);
        }
    }
}
