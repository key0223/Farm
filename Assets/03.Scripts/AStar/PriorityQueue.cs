using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> _heap = new List<T>();

    public int Count { get { return _heap.Count; } }

    public void Enqueue(T item)
    {
        _heap.Add(item);

        int i = _heap.Count - 1;

        while (i > 0)
        {
            int parent = (i-1) / 2;
            if (_heap[parent].CompareTo(_heap[i]) <= 0) break;
            Swap(parent, i);
            i = parent;
        }
    }

    public T Dequeue()
    {
        if(_heap.Count == 0) return default(T);

        T result = _heap[0];
        _heap[0] = _heap[_heap.Count-1];
        _heap.RemoveAt(_heap.Count-1);

        int i = 0;
        while(true)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            int smallest = i;

            /*  삽입 노드와 자식 노드를 비교하여 자식 노드 중 더 큰 값과 교환 */
            if(left< _heap.Count && _heap[left].CompareTo(_heap[smallest])<0)
                smallest = left;
            if (right < _heap.Count && _heap[right].CompareTo(_heap[smallest]) < 0)
                smallest = right;

            if (smallest == i) break;
            Swap(i, smallest);
            i = smallest;
        }

        return result;
    }

    public bool Contains(T item)
    {
        return _heap.Contains(item);
    }

    void Swap(int i, int j)
    {
        T temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;
    }
}
