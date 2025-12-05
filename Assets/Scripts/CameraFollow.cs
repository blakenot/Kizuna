using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;       
    [SerializeField] Vector3 offset = new Vector3(0, 6, -12);
    [SerializeField] float followSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition =new Vector3(0,0, player.position.z) + offset;
       
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        
        //transform.LookAt(player.position + Vector3.up * 1f); 
    }
}
