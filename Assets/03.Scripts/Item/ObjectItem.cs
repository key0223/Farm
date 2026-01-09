using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectItem : Item
{
    ObjectType _objectType;
    int _price;
    int _edibility;
    bool _isDrink;

    public ObjectType ObjectType {  get { return _objectType; }set { _objectType = value; } }
    public int Price { get { return _price; } set { _price = value; } }
    public int Edibility { get {return _edibility; } set { _edibility = value; } }
    public bool IsDrink {  get { return _isDrink; }set { _isDrink = value; } }
    public ObjectItem(ItemDataBase data, int stack)
    {
        ObjectDataBase objectData = data as ObjectDataBase;
        if(objectData == null)
            Debug.LogWarning($"Casting Failed. Id :{data.Id}, name : {data.Name} ");

        Id = objectData.Id;
        Name = objectData.Name;
        DisplayName = objectData.DisplayName;
        Description = objectData.Description;
        Category = objectData.Category;
        CategoryColor = objectData.CategoryColor;
        Stackable  = objectData.Stackable;
        CanBeGivenAsGift = objectData.CanBeGivenAsGift;
        CanBeTrashed = objectData.CanBeTrashed;
        Tags = Parser.ParseString(objectData.ContextTags);
        Icon = LoadUtils.GetAtlasSprite(data.SheetDirectory,data.ParentSheet,data.SpriteIndex);

        _objectType = objectData.ObjectType;
        _price = objectData.Price;
        _edibility = objectData.Edibility;
        _isDrink = objectData.IsDrink;

        Stack = stack;
    }
    public override int Stack { get { return _stack; } set { _stack = value; } }

    public override Item OnClone(Item clone)
    {
        /* ObjectItem 고유 필드 복사 */
        ObjectItem objectItem = clone as ObjectItem;
        objectItem.ObjectType = _objectType;
        objectItem.Price = _price;
        objectItem.Edibility = _edibility;
        objectItem._isDrink = _isDrink;
        Tags = new HashSet<string>(objectItem.Tags);

        return objectItem;
    }
}
