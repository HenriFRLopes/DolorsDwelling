using UnityEngine;
using System.Collections.Generic;

public class GameplayState : MonoBehaviour
{
    
    public bool gameplay_state;
    public GameObject pause_screen;
    public List<GameObject> options;

    private int id_option;
    private int previous_option;

    void Start(){
        id_option = 0;
        previous_option = 0;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            gameplay_state = gameplay_state ? false : true;
        }
        if(gameplay_state){
            pause_screen.SetActive(true);
            draw_option();
            option_navegation();
        }else{
            pause_screen.SetActive(false);
        }
    }

    void option_navegation(){
        if(Input.GetKeyDown("s")){ //substituir inputs pelos botões corretos
            id_option = (id_option+1)%options.Count;
        }else if(Input.GetKeyDown("w")){ //substituir inputs pelos botões corretos
            id_option = (id_option-1)%options.Count;
            if(id_option < 0) id_option = options.Count + id_option;
        }
        if(Input.GetKeyDown("enter")){
            options[id_option].GetComponent<Option>().set_option();
        }
    }

    void draw_option(){
        options[previous_option].GetComponent<Option>().selected = false;
        options[id_option].GetComponent<Option>().selected = true;
        previous_option = id_option;
    }
}
