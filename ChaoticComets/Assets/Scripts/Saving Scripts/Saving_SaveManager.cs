using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public static class Saving_SaveManager
{
    private static readonly string savename = "/current.save";

    public static void SaveData(GameManager gM, GameObject player1, GameObject player2) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + savename;
        FileStream stream = new FileStream(path, FileMode.Create);

        Saving_PlayerManager data = new Saving_PlayerManager(gM, player1, player2);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Saved to: " + Application.persistentDataPath + savename);
    }

    public static Saving_PlayerManager LoadData() {
        string path = Application.persistentDataPath + savename;
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Saving_PlayerManager data = formatter.Deserialize(stream) as Saving_PlayerManager;
            stream.Close();

            return data;
        }
        else { // Expected behaviour from main menu. Elsewhere, it may indicate user manually deleted it mid-game, and will be handled by GameManager
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void EraseData() {
        string path = Application.persistentDataPath + savename;
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }
    

}
