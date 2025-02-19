using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystem
{
    readonly static string SaveFile = "/SaveData.abc";
    public static void SaveData(GameData Gamedata) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, SaveFile);

        
        using(FileStream stream = new FileStream(path, FileMode.Create)) {
            formatter.Serialize(stream, Gamedata);
        }
    }

    public static GameData LoadSave() {
        string path = Path.Combine(Application.persistentDataPath, SaveFile);

        if (!File.Exists(path)) {
            Debug.LogWarning("Save file not found at " + path);
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        GameData data = null;

        using(FileStream stream = new FileStream(path, FileMode.Open)) {
            data = (GameData) formatter.Deserialize(stream);
        }

        return data;
    }
}
