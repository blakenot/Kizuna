using UnityEngine;

public class AnimController : MonoBehaviour
{
    public float speed = 5f;            
    public float horizontalSpeed = 5f;  

    void Update()
    {
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        
        float horizontal = Input.GetAxis("Horizontal");

        
        transform.Translate(Vector3.right * horizontal * horizontalSpeed * Time.deltaTime);
    }
}
