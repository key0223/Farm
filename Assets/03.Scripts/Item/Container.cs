using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Container 
{
    public event Action OnSlotChanged;
    public Storage Storage { get; private set; }

    public Container(int maxSlots)
    {
        Init(maxSlots);
    }

    void Init(int maxSlots)
    {
        Storage = new Storage(maxSlots);
    }

    public bool TryAdd(Item item)
    {
        if(Storage.HasSlot(item, out int slotIndex))
        {
            Storage.Add(slotIndex, item);
            OnSlotChanged?.Invoke();
            return true;
        }

        return false;
    }
    public bool TryRemove(int id, int count =1)
    {
        if(Storage.Remove(id,count))
        {
            OnSlotChanged?.Invoke();
            return true;
        }

        return false;
    }
    public void NotifyUIChanged()
    {
        OnSlotChanged?.Invoke();
    }
}
