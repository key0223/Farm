using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectItem : MonoBehaviour
{
    Item _item;

    GameLocation _location;
    Vector3Int _cellPos;
    bool _canPickup = false;

    SpriteRenderer _spriteRenderer;

    Coroutine _coPickupDelay;
    public Item Item { get { return _item; } }
    public Vector3Int CellPos { get { return _cellPos; } }
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(GameLocation gameLocation,Item newItem, Vector3Int worldPos)
    {
        _location = gameLocation;
        _item = newItem;
        _cellPos = worldPos;
        _spriteRenderer.sprite = _item.Icon;
        transform.position = new Vector3(worldPos.x, worldPos.y, 0);

        if(_coPickupDelay != null)
            StopCoroutine(_coPickupDelay);

        _coPickupDelay = StartCoroutine(CoDelayPickupEnable());
    }

    public bool TryPickup(PlayerInventory playerInventory)
    {
        if (!_canPickup || _item == null) return false;
        if(!playerInventory.TryAdd(_item)) return false;

        _location.RemoveWorldObjects(_cellPos, _item.Id);
        
        // TODO : 픽업 애니메이션
        return true;
    }

    IEnumerator CoDelayPickupEnable()
    {
        yield return new WaitForSeconds(1.0f);
        _canPickup = true;
    }

    void OnDestroy()
    {
        if (_coPickupDelay != null)
            StopCoroutine(_coPickupDelay);

    }
}
