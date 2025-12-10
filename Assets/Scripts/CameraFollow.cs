using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public ScreenBorderGlow borderGlow;
    public Vector3 firstPersonOffset = new Vector3(0f, 5f, 0.1f);

    
    public Vector3 thirdPersonOffset = new Vector3(0f, 5f, -10f);

    public bool useFirstPerson = true;  

    void LateUpdate()
    {
        if (player == null) return;

        if (useFirstPerson)
        {
           
            transform.position = player.position + firstPersonOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            
            Vector3 targetPos = new Vector3(
                player.position.x,
                player.position.y + thirdPersonOffset.y,
                player.position.z + thirdPersonOffset.z
            );

            transform.position = targetPos;
            transform.rotation = Quaternion.Euler(15f, 0f, 0f); // slight tilt downward
        }
    }

    public void ToggleView()
    {
        useFirstPerson = !useFirstPerson;
        if (borderGlow != null)
            borderGlow.SetVisible(useFirstPerson);
    }
}
