using UnityEngine;

[CreateAssetMenu(fileName = "Fala", menuName = "Scriptable Objects/Fala")]
public class Fala : ScriptableObject
{
    public string texto;
    public Fala nextFala;
    public bool hasResponse = false;
    public DialogueOption[] opcoes; 
}
