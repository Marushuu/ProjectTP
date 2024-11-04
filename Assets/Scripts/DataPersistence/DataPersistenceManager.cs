using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Configuration")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private FileDataHandler _fileDataHandler;
    private List<IDataPersistence> _dataPersistenceObjects;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Too many DataPersistenceManagers in the scene. Destroying the new one.");
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        _dataPersistenceObjects = FindDataPersistenceObjects();
        NewGameData();
    }

    public void NewGameData()
    {
        Debug.Log("Creating new game data.");
        gameData = new GameData();
    }

    public void LoadGameData()
    {
        gameData = _fileDataHandler.LoadGameData();

        if (gameData == null)
        {
            Debug.Log("No game data found. Creating new game data.");
            NewGameData();
        }

        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.LoadGameData(gameData);
        }

        Debug.Log("Game data loaded.");
    }

    public void SaveGameData()
    {
        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.SaveGameData(ref gameData);
        }

        _fileDataHandler.SaveGameData(gameData);

        Debug.Log("Game data saved.");
    }

    private List<IDataPersistence> FindDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> _dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(_dataPersistenceObjects);
    }
}
