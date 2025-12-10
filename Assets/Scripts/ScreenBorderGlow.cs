using UnityEngine;
using UnityEngine.UI;

public class ScreenBorderGlow : MonoBehaviour
{
    [SerializeField] Image glowImage;

    void Awake()
    {
        if (glowImage != null)
            glowImage.enabled = false;
    }

    public void SetGlowColor(Color c)
    {
        if (glowImage != null)
            glowImage.color = c;
    }

    public void SetVisible(bool v)
    {
        if (glowImage != null)
            glowImage.enabled = v;
    }
}
