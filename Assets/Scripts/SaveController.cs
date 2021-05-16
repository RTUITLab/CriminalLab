using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveController
{
    private const string gameDataFileName = "data.binary";
    public static void SaveData(NoteData[] noteDatas, ThreadData[] threadDatas)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Path.Combine(Application.persistentDataPath, gameDataFileName);
        FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

        try
        {
            GameData gameData = new GameData();
            gameData.NoteDatas = new NoteData[noteDatas.Length];
            gameData.ThreadDatas = new ThreadData[threadDatas.Length];

            noteDatas.CopyTo(gameData.NoteDatas, 0);
            threadDatas.CopyTo(gameData.ThreadDatas, 0);

            formatter.Serialize(stream, gameData);

            Debug.Log("Data was saved");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.StackTrace + "\n\n" + ex.Message);
        }
        stream.Close();
    }

    public static GameData LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, gameDataFileName);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            try
            {
                GameData gameData = (GameData)formatter.Deserialize(stream);
                Debug.Log("Data was loaded");
                return gameData;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.StackTrace + "\n\n" + ex.Message);
            }
            stream.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.StackTrace + "\n\n" + ex.Message);
        }
        return null;
    }
}
