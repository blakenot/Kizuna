using UnityEngine;
using UnityEngine.UI;

public class ScreenBorderGlow : MonoBehaviour
{
    [SerializeField] Image glowImage;

    [Header("Glow Settings")]
    [SerializeField] float minAlpha = 0.15f;
    [SerializeField] float maxAlpha = 0.35f;
    [SerializeField] float pulseSpeed = 1.2f;
    [SerializeField] bool usePulse = true;

    Color baseColor;
    bool isVisible = false;

    void Awake()
    {
        if (glowImage != null)
        {
            glowImage.enabled = false;
            baseColor = glowImage.color;
        }
    }

    void Update()
    {
        if (!isVisible || glowImage == null)
            return;

        if (!usePulse)
            return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        glowImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }

    public void SetGlowColor(Color c)
    {
        if (glowImage == null) return;

        if (c.b > c.r && c.b > c.g)
        {
            c = new Color(0.3f, 0.6f, 1f, c.a);
        }

        baseColor = c;

        glowImage.color = new Color(c.r, c.g, c.b, (minAlpha + maxAlpha) * 0.5f);
    }

    public void SetVisible(bool v)
    {
        isVisible = v;

        if (glowImage != null)
            glowImage.enabled = v;
    }
}
