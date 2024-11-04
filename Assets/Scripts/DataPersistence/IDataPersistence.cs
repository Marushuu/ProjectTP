using UnityEngine;

public interface IDataPersistence
{
    void LoadGameData(GameData data);

    void SaveGameData(ref GameData data);
}
