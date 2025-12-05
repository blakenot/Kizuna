using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public TrackSegment.ColorState obstacleColor;
    [SerializeField] Renderer visual;

    public void Initialize(TrackSegment.ColorState color)
    {
        obstacleColor = color;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (!visual) return;
        visual.material.color = obstacleColor switch
        {
            TrackSegment.ColorState.Red => Color.red,
            TrackSegment.ColorState.Green => Color.green,
            TrackSegment.ColorState.Blue => Color.blue,
            TrackSegment.ColorState.Black => Color.black,
            _ => Color.white
        };
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (!player) return;

        if (obstacleColor == TrackSegment.ColorState.Black)
        {
            GameManager.Instance.Fail("Hit black obstacle");
            return;
        }

        bool match =
            (obstacleColor == TrackSegment.ColorState.Red && player.CurrentColor == PlayerController.ColorState.Red) ||
            (obstacleColor == TrackSegment.ColorState.Green && player.CurrentColor == PlayerController.ColorState.Green) ||
            (obstacleColor == TrackSegment.ColorState.Blue && player.CurrentColor == PlayerController.ColorState.Blue);

        if (!match)
        {
            GameManager.Instance.Fail("Hit mismatched obstacle");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
