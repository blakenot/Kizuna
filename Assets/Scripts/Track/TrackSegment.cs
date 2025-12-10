using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    public enum ColorState { White, Red, Green, Blue, Black }
    [SerializeField] ColorState tileColor;
    Vector3 tilePosition;
    public ColorState TileColor => tileColor;

    void Awake()
    {
        DetectColorFromPrefabName();
    }

    public void Initialize(Vector3 tilePosition)
    {
        this.tilePosition = tilePosition;
    }

    void DetectColorFromPrefabName()
    {
        string n = gameObject.name.ToLower();
        if (n.Contains("white")) tileColor = ColorState.White;
        else if (n.Contains("red")) tileColor = ColorState.Red;
        else if (n.Contains("green")) tileColor = ColorState.Green;
        else if (n.Contains("blue")) tileColor = ColorState.Blue;
        else if (n.Contains("black")) tileColor = ColorState.Black;
        else tileColor = ColorState.White;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) ValidatePlayer(player.CurrentColor);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) ValidatePlayer(player.CurrentColor);
        }
    }

    void ValidatePlayer(PlayerController.ColorState playerColor)
    {
        if (tileColor == ColorState.White) return;
        if (tileColor == ColorState.Black)
        {
            if (GameManager.Instance != null) GameManager.Instance.Fail("Black tile");
            return;
        }
        if (!ColorsMatch(playerColor))
        {
            if (GameManager.Instance != null) GameManager.Instance.Fail("Color mismatch");
        }
    }

    bool ColorsMatch(PlayerController.ColorState player)
    {
        return (tileColor == ColorState.Red && player == PlayerController.ColorState.Red)
               || (tileColor == ColorState.Green && player == PlayerController.ColorState.Green)
               || (tileColor == ColorState.Blue && player == PlayerController.ColorState.Blue);
    }
}
