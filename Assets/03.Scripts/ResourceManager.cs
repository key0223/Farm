using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMonobehaviour<ResourceManager>
{
    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.ManagerReady("ResourceManager");
    }

    public T Load<T> (string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if(index >= 0)
                name = name.Substring(index + 1);

            /* 만들어진게 있으면 풀에서 가져옴 */
            GameObject obj = PoolManager.Instance.GetOriginal(name);
            if (obj != null)
                return obj as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if(original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return PoolManager.Instance.Pop(original, parent).gameObject;

        /* 풀링 대상 아니면 */
        GameObject obj = Object.Instantiate(original, parent);
        obj.name = original.name;
        return obj;
    }

    public void Destroy(GameObject obj)
    {
        if (obj == null) return;

        /* 풀링 대상 */
        Poolable poolable = obj.GetComponent<Poolable>();
        if(poolable != null)
        {
            PoolManager.Instance.Push(poolable);
            return;
        }

        Object.Destroy(obj);
    }
}
