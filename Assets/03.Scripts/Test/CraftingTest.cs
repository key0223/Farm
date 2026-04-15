using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingTest : MonoBehaviour
{
    RecipeRequirementEvaluator _evaluator;
    Container _container;


    void Awake()
    {
        _evaluator = new RecipeRequirementEvaluator();
        _container = new Container(36);
    }

    void Start()
    {
        Invoke("RunAllTests",1f);
    }

    void RunAllTests()
    {
        Init_Container();

        List<Recipe> recipes = CreateRecipes();

        foreach (Recipe recipe in recipes)
        {
            bool canMake = _evaluator.CanMake(recipe.Needs, _container);
            Debug.Log($"{recipe.ResultItemId}: {canMake}");
        }
    }

  
    void Init_Container()
    {
        AddItem(301, 10); // milk
        //AddItem(302, 10); // carrot
        AddItem(303, 10); // cauliflower
        AddItem(304, 10); // pumpkin
        AddItem(307, 10); // parsnip
        AddItem(308, 10); // potato
        AddItem(311, 10); // wheat
        AddItem(316, 10); // egg
        AddItem(324, 10); // salt
        AddItem(325, 10); // oil
        AddItem(326, 10); // butter
    }
    List<Recipe> CreateRecipes()
    {
        List<Recipe> recipes = new();

        recipes.Add(new Recipe(801)); // Veggie
        recipes.Add(new Recipe(802)); // Bread
        recipes.Add(new Recipe(803)); // Potato
        recipes.Add(new Recipe(804)); // Muffin
        recipes.Add(new Recipe(805)); // Parsnip

        return recipes;
    }

    void AddItem(int id, int count)
    {
        Item item = ItemFactory.Create(id);
        _container.TryAdd(item);
    }
}
