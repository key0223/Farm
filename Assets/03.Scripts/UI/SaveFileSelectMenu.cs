using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.VFX;

public class SaveFileSelectMenu : MonoBehaviour
{
    string _saveFileSlotPrefabPath = "UI/SaveFileSlot";
    void SetSaveFileSlots()
    {
        List<string> saveFiles = SaveLoadManager.Instance.GetAllSaveFiles();

        foreach (string saveFile in saveFiles)
        {
            GameSave gameSave = SaveLoadManager.Instance.LoadGameSave(saveFile);
            if (gameSave == null)
                continue;

            GameObject slotObj = ResourceManager.Instance.Instantiate(_saveFileSlotPrefabPath);
        }
    }
}
