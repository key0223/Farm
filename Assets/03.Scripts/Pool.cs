using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool
{
    public GameObject Original { get; private set; } /* 원본 프리팹 */
    public Transform Root { get; private set; } /* 풀 이름, 풀링에 사용할 오브젝트 부모*/

    Stack<Poolable> _poolStack = new Stack<Poolable>();

    public void Init(GameObject original, int count = 5)
    {
        Original = original;

        /* Root 생성 */
        Root = new GameObject().transform;
        Root.name = $"{original.name}_Root";

        for (int i = 0; i < count; i++)
            Push(Create());
        
    }

    Poolable Create()
    {
        GameObject obj = Object.Instantiate<GameObject>(Original);
        obj.name = Original.name;
        return obj.GetOrAddComponent<Poolable>();
    }

    public void Push(Poolable poolable)
    {
        if (poolable == null) return;

        poolable.transform.SetParent(Root,false);
        poolable.gameObject.SetActive(false);
        poolable._isUsing = false;

        _poolStack.Push(poolable);
    }

    public Poolable Pop(Transform parent)
    {
        Poolable poolable;

        if(_poolStack.Count > 0)
            poolable = _poolStack.Pop();
        else
            poolable = Create();

        poolable.gameObject.SetActive(true);

        // DontDestroyOnLoad 해제
       if(parent == null)
            poolable.transform.SetParent(MapManager.Instance.ItemsParent,false);

        poolable.transform.SetParent(parent,false);
        poolable._isUsing = true;

        return poolable;
    }
}
