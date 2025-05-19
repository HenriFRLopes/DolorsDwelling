using UnityEngine;

public class CamMove : MonoBehaviour
{
    public Transform player;

    void FixedUpdate(){
        Vector3 dir = new Vector3(5*(player.position.x - transform.position.x), 5*(player.position.y + 5 - transform.position.y), 0);
        //transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, -10), 20 * Time.deltaTime);
        transform.Translate(dir * Time.deltaTime);
    }
}
