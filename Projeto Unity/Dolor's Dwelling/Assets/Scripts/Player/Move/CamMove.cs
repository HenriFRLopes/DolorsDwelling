using UnityEngine;

public class CamMove : MonoBehaviour
{
    public Transform player;

    void FixedUpdate(){
        transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, -10), 3 * Time.deltaTime);
    }
}
