using System.Collections.Generic;
using UnityEngine;
using static Define;

[SerializeField]
public class RecipeDataBase 
{
    public int Id;
    public RecipeType RecipeType;
    public string Req_ingredient;
    public int ResultItemId;

    public List<Need> Needs;
}
