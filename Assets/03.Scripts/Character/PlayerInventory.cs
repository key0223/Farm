using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Container _container;

    int _currentToolbarIndex = -1;

    public Container PlayerContainer {  get { return _container; } }
    public int CurrentToolbarIndex {  get { return _currentToolbarIndex; } }  

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
        Item shovel = ItemFactory.Create(7040);
        TryAdd(shovel);

        Item wateringCan = ItemFactory.Create(7050);
        TryAdd(wateringCan);

        Item ore = ItemFactory.Create(318);
        TryAdd(ore);

        Item seed = ItemFactory.Create(611);
        TryAdd(seed);
        //ObjectItem objectItem = item as ObjectItem;
        //MapManager.Instance.CurrentLocation.AddWorldObject(objectItem, new Vector3Int(11, -7, 0));
    }
   
    public bool TryAdd(Item item)
    {
        return _container.TryAdd(item);
    }
    public bool TryRemove(int id, int count)
    {
        if(count <= 0) return false;

        bool removed = _container.TryRemove(id, count);

        return removed;
    }
}
