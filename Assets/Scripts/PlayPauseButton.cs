using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButton : MonoBehaviour
{
    public Sprite playSprite;
    public Sprite pauseSprite;

    private Image buttonImage;
    private bool isPaused = false;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = playSprite; 
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;           
            buttonImage.sprite = pauseSprite;
        }
        else
        {
            Time.timeScale = 1f;            
            buttonImage.sprite = playSprite;
        }
    }
}
