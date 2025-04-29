using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RoomControl : MonoBehaviour
{
    public int id_room;
    public Animator fade;
    
    private static int id_previous_room;
    private string next_room;

    [System.Serializable]
    public struct SpawnPosition{
        public int previous_room;
        public Transform spawn_point;
    }

    public SpawnPosition[] points;
    public Dictionary<int, Transform> player_start_positions;

    void Start()
    {
        player_start_positions = new();
        foreach(SpawnPosition p in points){
            player_start_positions.Add(p.previous_room, p.spawn_point);
        }
        if(id_previous_room != 0) GameObject.FindGameObjectWithTag("Player").transform.position = player_start_positions[id_previous_room].position;
    }

    public void GoRoom(string room){
        id_previous_room = id_room;
        next_room = room;
        fade.SetInteger("fade", 1);
        Invoke("LoadScene", 1f);
    }

    private void LoadScene(){
        SceneManager.LoadScene(next_room);
    }
}
