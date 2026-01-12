using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField] string _guid = "";
    public string GUID { get { return _guid; } set { _guid = value; } }

    void Awake()
    {
        if(!Application.IsPlaying(gameObject))
        {
            if(_guid =="")
                _guid = System.Guid.NewGuid().ToString();
        }
        
    }
}
