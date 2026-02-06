using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "so_PortalDatas", menuName = "Scriptable Objects/Portal Datas")]
public class SO_PortalData : ScriptableObject
{
    [SerializeField] public List<Portal> FarmHouse;
    [SerializeField] public List<Portal> Farm;
}
