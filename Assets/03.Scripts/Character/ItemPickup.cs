using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemPickup : MonoBehaviour
{
    PlayerController _playerController;
    GameLocation _currentLocation;
    Vector3Int _playerCellPos;
    void Awake()
    {
        GameManager.OnAllManagersReady += SubscribeEvent;
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        _currentLocation = MapManager.Instance.CurrentLocation;
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        MapManager.Instance.OnLocationChanged -= SetCurrentLocation;
        MapManager.Instance.OnLocationChanged += SetCurrentLocation;
    }
    void OnDisable()
    {
        MapManager.Instance.OnLocationChanged -= SetCurrentLocation;
    }
    void SubscribeEvent()
    {
        MapManager.Instance.OnLocationChanged += SetCurrentLocation;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }


    public void CheckAutoPickup(Vector3Int playerCellPos)
    {
        if (_currentLocation == null || _playerController.PlayerInven == null) return;

        _playerCellPos = playerCellPos;

        List<WorldObjectItem> nearbyItems = _currentLocation.GetPickupableItem(playerCellPos, range: 3);

        foreach (WorldObjectItem worldItem in nearbyItems)
        {
            if (worldItem.TryPickup(_playerController.PlayerInven))
            {
                SoundManager.Instance.PlaySound(SoundName.EFFECT_PICKUP);
            }
        }
    }
    void SetCurrentLocation(GameLocation currentLoaction)
    {
        _currentLocation = currentLoaction;
    }
}
