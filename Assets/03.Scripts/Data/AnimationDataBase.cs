using SuperTiled2Unity.Ase.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
[Serializable]
public class AnimationDataBase 
{
    public string AnimationName;
    public SpriteLayer LayerType;
    public string SheetDirectory;
    public string ParentSheet;
    public string SpriteIndexStr;
    public bool Loop;

    public List<int> SpriteIndex;
    
}
