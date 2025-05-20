using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour // Controle de Dados
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData; // Dados

    private List<IDataHolder> dataHolderObjects;

    private FileDataHandler dataHandler;

    public static DataManager instance { get; private set; }

// Inicia o Data Manager e confere se existe somente um na cena
    private void Awake() {
       
       if (instance != null)
       {
            Debug.LogError("Multiple Data Managers Detected in the Scene");
       }
        instance = this;
    }

    private void Start() // Chamar quando o jogo iniciar
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataHolderObjects = FindAllDataHolderObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load(); // Carrega os dados

        if (this.gameData == null) // Checa se existe o arquivo de salvamento.
        {
            Debug.Log("No Game Data, creating new Game Data");
            NewGame();
        }

        foreach (IDataHolder dataHObj in dataHolderObjects)
        {
            dataHObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        foreach (IDataHolder dataHObj in dataHolderObjects)
        {
            dataHObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData); // Salva os dados
    }

    private void OnApplicationQuit() // Salva quando o jogo fecha
    {
        SaveGame();
    }

    private List<IDataHolder> FindAllDataHolderObjects()
    {
        IEnumerable<IDataHolder> dataHolderObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataHolder>();

        return new List<IDataHolder>(dataHolderObjects);
    }
}
