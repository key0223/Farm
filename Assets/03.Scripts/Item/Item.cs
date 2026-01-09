using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/* 모든 아이템, 인벤토리 슬롯에 들어가는 모든 객체의 공통 속성(이름, 가격, 스프라이트 정의) */
/* 오브젝트는 Item을 상속하며 아이템 타입(채소,광물,씨앗 등)으로 세계에 배치,수거,먹기, 판매 가능. */
public abstract class Item
{
    int _id;
    string _name;
    string _displayName;
    string _description;
    string _category;
    string _categoryColor;
    bool _stackable = false;
    bool _canBeGivenAsGift;
    bool _canBeTrashed;

    HashSet<string> _tags = new HashSet<string>();
    Sprite _icon;
    protected int _stack;

    public int Id { get { return _id; } set { _id = value; } }
    public string Name { get { return _name; } set { _name = value; } }
    public string DisplayName { get { return _displayName; } set { _displayName = value; } }
    public string Description { get { return _description; }set { _description = value; } }
    public string Category { get { return _category; } set { _category = value; } } 
    public string CategoryColor { get { return _categoryColor; } set { _categoryColor = value; } }  
    public bool Stackable { get { return _stackable; } set { _stackable = value; } }

    public bool CanBeGivenAsGift { get { return _canBeGivenAsGift; } set { _canBeGivenAsGift = value; } }
    public bool CanBeTrashed { get { return _canBeTrashed; } set { _canBeTrashed = value; } }
    public HashSet<string> Tags { get { return _tags; }  set { _tags = value; } }
    public abstract int Stack { get; set; }
    public Sprite Icon { get { return _icon; } set { _icon = value; } }

    public Item Clone()
    {
        Item clone = MemberwiseClone() as Item;
        clone.Stack = Stack;
        return OnClone(clone);
    }
    public abstract Item OnClone(Item clone);
}
