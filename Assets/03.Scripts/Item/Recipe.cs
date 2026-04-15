using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static Define;

public class Recipe
{
    int _id;
    int _resultItemId;
    RecipeType _recipeType;
    List<Need> _needs = new List<Need>();

    public int ResultItemId { get { return _resultItemId; } }
    public List<Need> Needs { get { return _needs; } }
    public Recipe(int cookingId)
    {
        int recipeId =  TableDataManager.Instance.RecipeDict.Values.FirstOrDefault(x=> x.ResultItemId == cookingId).Id;

       TableDataManager.Instance.RecipeDict.TryGetValue(recipeId, out RecipeDataBase recipeData);
       
        _id = recipeData.Id;
        _resultItemId = recipeData.ResultItemId;
        _recipeType = recipeData.RecipeType;

        _needs = Parser.ParserNeeds(recipeData.Rep_ingredient);
    }
    
}
