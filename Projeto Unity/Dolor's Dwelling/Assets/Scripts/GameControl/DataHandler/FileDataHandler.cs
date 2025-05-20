using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class FileDataHandler // Armazena e recupera informações salvas em arquivos fora do jogo.
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        // Combina o caminho para o arquivo com o nome do arquivo de salvamento
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {

                string dataJSON = "";

                // Cria o arquivo e armazena os dados
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataJSON = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataJSON);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR: Could not load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // Combina o caminho para o arquivo com o nome do arquivo de salvamento
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        try
        {
            // Cria o diretório
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Transforma os dados em JSON
            string dataJSON = JsonUtility.ToJson(data, true);

            // Cria o arquivo e armazena os dados
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataJSON);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("ERROR: Could not save data to file: " + fullPath + "\n" + e);
        }
    }
}
