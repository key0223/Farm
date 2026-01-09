using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Storage
{
    Item[] _slots;

    public Storage(int maxSlots)
    {
        _slots = new Item[maxSlots];
    }
    public int Stack(int idx)
    {
        if(idx<0 || idx >= _slots.Length || _slots[idx] == null)
            return 0;

        return _slots[idx].Stack;
    }
    public Item GetItemAtSlot(int idx)
    {
        return (idx >= 0) && idx < _slots.Length ? _slots[idx] : null;
    }
    public int GetSlotIndex(int itemId)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            Item item = _slots[i];
            if (item != null && item.Id == itemId)
                return i;
        }

        return -1;
    }
    public void SetItemAtSlot(int idx,Item item)
    {
        if(idx >= 0 && idx< _slots.Length)
            _slots[idx] = item;
    }

    public void SwapOrMerge(int targetIdx,Item draggedItem, int sourceIdx, Container sourceContainer = null)
    {
        Item targetItem = GetItemAtSlot(targetIdx);

        if(targetItem == null)
        {
            /* 빈 슬롯일 경우 그냥 놓기 */
            SetItemAtSlot(targetIdx, draggedItem);
            if(sourceIdx >=0 && sourceContainer != null)
                sourceContainer.Storage.ClearSlot(sourceIdx);
        }
        else if(targetItem.Id == draggedItem.Id && draggedItem.Stackable && targetItem.Stackable)
        {
            /* 같은 아이템일 경우 스택 합침 (남는 스택만큼)  */
            int spaceLeft = Define.ITEM_MAX_STACK - targetItem.Stack;
            int transferAmount = Mathf.Min(draggedItem.Stack, spaceLeft);

            targetItem.Stack += transferAmount;

            if(draggedItem.Stack > transferAmount)
            {
                draggedItem.Stack -= transferAmount;
                if(sourceContainer != null)
                    sourceContainer.Storage.SetItemAtSlot(sourceIdx,draggedItem);
            }
        }
        else
        {
            /* 교환 */
            Item targetItemClone = targetItem.Clone();
            
            SetItemAtSlot(targetIdx,draggedItem.Clone());

            if(sourceContainer.Storage != null)
                sourceContainer.Storage.SetItemAtSlot(sourceIdx,targetItemClone);
        }
    }
    public void Add(int idx, Item item)
    {
        if (item == null || idx < 0 || idx >= _slots.Length) return;

        if (_slots[idx] == null)
            _slots[idx] = item.Clone();
        else
            _slots[idx].Stack += item.Stack;
    }
    
    public bool Remove(int id, int count)
    {
        if (count <= 0) return true;

        int idx = GetSlotIndex(id);

        if (idx != -1)
        {
            RemoveAt(idx,count);
            return true;
        }

        return false;
    }

    public void RemoveAt(int idx, int count = 1)
    {
        if (idx < 0 || idx >= _slots.Length || _slots[idx] == null) return;

        int curStack = Stack(idx);
        int left = Mathf.Max(0, curStack - count);

        if (left == 0)
            _slots[idx] = null;
        else
            _slots[idx].Stack = left;

    }

    public bool HasSlot(Item item, out int assignedSlotIndex)
    {
        assignedSlotIndex = -1;

        if (item.Stackable)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null && _slots[i].Id == item.Id && _slots[i].Stack < Define.ITEM_MAX_STACK)
                    return true;
            }
        }

        /* 빈 슬롯 */
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                assignedSlotIndex = i;
                return true;
            }
        }

        return false;
    }
    public bool ConsumeExact(int id, int count)
    {
        return Remove(id, count);
    }

    void ClearSlot(int slotIdx)
    {
        if (slotIdx < 0 || slotIdx >= _slots.Length)
            return;

        SetItemAtSlot(slotIdx, null);
    }
}
