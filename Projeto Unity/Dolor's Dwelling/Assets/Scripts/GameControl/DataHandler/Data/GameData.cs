using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData // Segura as informações do jogo
{
    public bool hasDash;
    public bool hasWallJump;


// Inicia os valores inicais na criação de um novo jogo.
    public GameData() {
        this.hasDash = false;
        this.hasWallJump = false;
    }
}
