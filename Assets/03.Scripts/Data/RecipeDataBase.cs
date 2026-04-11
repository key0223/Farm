using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[SerializeField]
public class RecipeDataBase 
{
    public int Id;
    public RecipeType RecipeType;
    public int Rep_ingredient;
    public int ResultItemId;

    public List<Need> Needs;

}
