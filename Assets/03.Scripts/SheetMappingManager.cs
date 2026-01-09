using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
public class SheetVariant
{
    public string BaseSheetName;
    public string[] Variants;
    public string[] ActualSheets;
}
public class SheetMappingManager : SingletonMonobehaviour<SheetMappingManager>
{
   [SerializeField] SheetVariant[] _mappings;

    protected override void Awake()
    {
        base.Awake();
        Init();
        GameManager.Instance.ManagerReady("SheetMappingManager");
    }

    void Init()
    {
        Init_ActualSheet();
    }

    void Init_ActualSheet()
    {
        for (int i = 0; i < _mappings.Length; i++)
        {
            _mappings[i].ActualSheets = new string[_mappings[i].Variants.Length];

            for (int j = 0; j < _mappings[i].Variants.Length; j++)
            {
                _mappings[i].ActualSheets[j] = $"{_mappings[i].BaseSheetName}_{_mappings[i].Variants[j]}";
            }
        }
    }

    public bool GetActualSheet(string baseSheet, string variant, out string actualSheet)
    {
        foreach(SheetVariant mapping in _mappings)
        {
            if(mapping.BaseSheetName == baseSheet)
            {
                for(int i = 0;i < mapping.Variants.Length; i++)
                {
                    if (mapping.Variants[i] == variant)
                    {
                        actualSheet = mapping.ActualSheets[i];
                        return true;
                    }
                }
            }
        }

        actualSheet = null;
        return false;
    }
}
