using System.Collections.Generic;
using static Define;

public class Recipe
{
    int _id;
    int _resultItemId;
    RecipeType _recipeType;
    List<Need> _needs = new List<Need>();

    public int ResultItemId { get { return _resultItemId; } }
    public List<Need> Needs { get { return _needs; } }
    public Recipe(int recipeId)
    {
        RecipeDataBase data = TableDataManager.Instance.RecipeDict[recipeId];

        _id = data.Id;
        _resultItemId = data.ResultItemId;
        _recipeType = data.RecipeType;

        _needs = Parser.ParserNeeds(data.Rep_ingredient);
    }
    
}
