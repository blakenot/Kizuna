using UnityEngine;

public class AnimController : MonoBehaviour
{
    public float speed = 5f;            // Forward speed
    public float horizontalSpeed = 5f;  // Side movement speed

    void Update()
    {
        // Move forward constantly
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Get horizontal input (A/D or LeftArrow/RightArrow)
        float horizontal = Input.GetAxis("Horizontal");

        // Move left/right
        transform.Translate(Vector3.right * horizontal * horizontalSpeed * Time.deltaTime);
    }
}
