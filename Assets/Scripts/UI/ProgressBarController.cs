using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] Slider progressSlider;

    [Header("Orb Goal")]
    [SerializeField] int orbGoal = 100;

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

       
        int orbsCollected = GameManager.Instance.OrbsCollected;

       
        float orbProgress = Mathf.Clamp01((float)orbsCollected / orbGoal);

        
        progressSlider.value = orbProgress;
    }
}
