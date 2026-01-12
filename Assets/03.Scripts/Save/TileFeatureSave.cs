using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class TileFeatureSave 
{
    public TileFeatureType FeatureType;
    public int GridX;
    public int GridY;
    public int[] Data;
}

