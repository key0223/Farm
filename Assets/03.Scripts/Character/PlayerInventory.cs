using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Container _container;

    int _currentToolbarIndex = -1;

    Dictionary<int,bool> _unlockedRecipes = new Dictionary<int,bool>();
    public Container PlayerContainer {  get { return _container; } }
    public int CurrentToolbarIndex {  get { return _currentToolbarIndex; } }  

    public Dictionary<int, bool> UnlockedRecipes {  get { return _unlockedRecipes; } }
    public void SetCurrentToolbarItem(Item item)
    {
        _currentToolbarIndex = item != null ? _container.Storage.GetSlotIndex(item.Id) :-1;
    }
    void Awake()
    {
        _container = new Container(36);
    }

    void Start()
    {
        //Item shovel = ItemFactory.Create(7040);
        //TryAdd(shovel);

        //Item wateringCan = ItemFactory.Create(7050);
        //TryAdd(wateringCan);

        //Item ore = ItemFactory.Create(318);
        //TryAdd(ore);

        //Item seed = ItemFactory.Create(611);
        //TryAdd(seed);
        //ObjectItem objectItem = item as ObjectItem;
        //MapManager.Instance.CurrentLocation.AddWorldObject(objectItem, new Vector3Int(11, -7, 0));

        //Invoke("Init_Ingredients", 15f);
        //Invoke("Init_Recipes", 15f);
    }
   
    public bool TryAdd(Item item)
    {
        if(item.Category == "Recipe")
        {
            _unlockedRecipes[item.Id] = true;
            return true;
        }
        else
            return _container.TryAdd(item);
    }
    public bool TryAddAt(int idx, Item item)
    {
        return _container.TryAddAt(idx, item);
    }
    public bool TryRemove(int id, int count)
    {
        if(count <= 0) return false;

        bool removed = _container.TryRemove(id, count);

        return removed;
    }

    public bool HasRecipe(int recipeId)
    {
        bool hasRecipe = _unlockedRecipes.ContainsKey(recipeId);

        return hasRecipe;
    }
    /* Test */

    void Init_Ingredients()
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

    void Init_Recipes()
    {
        AddItem(801, 1);
        AddItem(802, 1);
        AddItem(803, 1);
        AddItem(804, 1);
        AddItem(805, 1);
    }
    void AddItem(int id,int count)
    {
        Item item = ItemFactory.Create(id,count);
        TryAdd(item);
    }
}
