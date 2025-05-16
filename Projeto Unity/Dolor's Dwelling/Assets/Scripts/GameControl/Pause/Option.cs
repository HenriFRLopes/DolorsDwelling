using UnityEngine;

public class Option : MonoBehaviour
{

    public bool selected;
    public GameObject arrow;

    void Start(){
        selected = false;    
    }

    void Update(){
        arrow.SetActive(selected);
    }

    public void set_option(){} //fazer com que essa função dispare um sinal para o submenu da opção selecionada;
}
