using UnityEngine;

public class MagicalOrb : MonoBehaviour
{
    [SerializeField] int orbValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.CollectOrb(orbValue);

        Destroy(gameObject);
    }
}
