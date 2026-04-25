using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewDialogueTable", menuName = "Scriptable Objects/Dialogue/Table")]

public class DialogueTable : ScriptableObject
{
    public string md5;
    [Header("Save Settings")]
    public string savePath = "Assets/Resources/Data/";
    public string fileName = "DialogueData_New.json";
    public List<DialogueData> array = new List<DialogueData>();
}
