using UnityEngine;

public class TriggerNextRoom : MonoBehaviour
{

    public string next_room_name;
    public RoomControl room_control;

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.CompareTag("Player")){
            room_control.GoRoom(next_room_name);
        }
    }
}
