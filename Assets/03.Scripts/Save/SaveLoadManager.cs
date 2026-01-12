using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{

    GameSave _gameSave;
    List<ISaveable> _iSaveableList;

    public List<ISaveable> ISaveableList { get { return _iSaveableList; } }
    protected override void Awake()
    {
        base.Awake();

        _iSaveableList = new List<ISaveable>();
        GameManager.Instance.ManagerReady("SaveLoadManager");
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if(File.Exists(Application.persistentDataPath + "/LifeIsSoup.dat"))
        {
            _gameSave = new GameSave();
            FileStream file = File.Open(Application.persistentDataPath + "/LifeIsSoup.dat", FileMode.Open);
            _gameSave = (GameSave)bf.Deserialize(file);

            for (int i = _iSaveableList.Count - 1; i >-1; i--)
            {
                if (_gameSave.GameObjectData.ContainsKey(_iSaveableList[i].ISaveableUniqueId))
                    _iSaveableList[i].ISaveableLoad(_gameSave);
                else
                {
                    Component component = (Component)_iSaveableList[i];
                    Destroy(component.gameObject);
                }
            }

            file.Close();
        }
    }

    public void SaveDataToFile()
    {
        _gameSave = new GameSave();

        foreach(ISaveable iSaveableObject in _iSaveableList)
        {
            _gameSave.GameObjectData.Add(iSaveableObject.ISaveableUniqueId, iSaveableObject.ISaveableSave());
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/LifeIsSoup.dat", FileMode.Create);
        bf.Serialize(file, _gameSave);

        file.Close();
    }

    public void StoreCurrentSceneData()
    {
        foreach(ISaveable iSaveableObject in _iSaveableList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }
    public void RestoreCurrentSceneData()
    {
        foreach (ISaveable iSaveableObject in _iSaveableList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
