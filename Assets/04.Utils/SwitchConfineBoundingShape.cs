using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
    }
  
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        GameSceneManager.Instance.OnAfterSceneLoad -= SwitchBoundingShape;
        GameSceneManager.Instance.OnAfterSceneLoad += SwitchBoundingShape;
    }

    void OnDisable()
    {
        GameSceneManager.Instance.OnAfterSceneLoad -= SwitchBoundingShape;
    }
    void SubscribeEvent()
    {
        GameSceneManager.Instance.OnAfterSceneLoad += SwitchBoundingShape;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }

    void SwitchBoundingShape()
    {
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        cinemachineConfiner.InvalidatePathCache();
    }
}
