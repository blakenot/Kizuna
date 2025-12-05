using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] Slider progressSlider;
    [SerializeField] Transform player;

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        float currentZ = player.position.z;
        float maxZ = GameManager.Instance.levelLength;

        float progress = Mathf.Clamp01(currentZ / maxZ);

        progressSlider.value = progress;
    }
}
