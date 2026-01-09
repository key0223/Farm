using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDragManager : SingletonMonobehaviour<GlobalDragManager>
{
    Item _draggedItem;
    Container _sourceContainer;
    int _sourceSlot;
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.ManagerReady("GlobalDragManager");
    }

    public void StartDrag(Item item, Container sourceContainer, int slot)
    {
        _draggedItem = item;
        _sourceContainer = sourceContainer;
        _sourceSlot = slot;
    }

    public void EndDrag(Container sourceContainer, int sourceSlot, Container targetContainer, int targetSlot)
    {
        if (_draggedItem == null) return;
        if (targetContainer != null && targetSlot != -1)
        {
            if (_sourceContainer != null && _sourceContainer.Storage != null)
            {
                /* 취소 : 원래 위치 복귀 */
                _sourceContainer.Storage.SetItemAtSlot(sourceSlot, _draggedItem);
            }

            /* 컨테이너 간 스왑 또는 병합 */
            targetContainer.Storage.SwapOrMerge(targetSlot, _draggedItem, sourceSlot, _sourceContainer);
            sourceContainer?.NotifyUIChanged();
            targetContainer?.NotifyUIChanged();
        }
        else /* 월드 드랍 */
        {
            DropNearPlayer(_draggedItem, sourceContainer, sourceSlot);
        }

        _draggedItem = null;
        _sourceContainer = null;
        _sourceSlot = -1;
    }

    void DropNearPlayer(Item item, Container sourceContainer, int sourceSlot)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        Vector3Int playerCellPos = player.CellPos;
        GameLocation currentLocation = MapManager.Instance.CurrentLocation;

        Vector3Int[] nearbyOffsets = {
        new Vector3Int(1, 0, 0),   // 오른쪽
        new Vector3Int(-1, 0, 0),  // 왼쪽  
        new Vector3Int(0, 1, 0),   // 아래
        new Vector3Int(0, -1, 0),  // 위
        new Vector3Int(1, 1, 0),   // 오른쪽 아래
        new Vector3Int(1, -1, 0),  // 오른쪽 위
        new Vector3Int(-1, 1, 0),  // 왼쪽 아래
        new Vector3Int(-1, -1, 0)  // 왼쪽 위
    };
        foreach (Vector3Int offset in nearbyOffsets)
        {
            Vector3Int dropPos = playerCellPos + offset;

            // 맵 범위 내에만 확인
            if (currentLocation.MapData.IsValidPosition(dropPos.x, dropPos.y))
            {
                currentLocation.AddWorldObject(item, dropPos);

                if (sourceContainer != null)
                    sourceContainer.NotifyUIChanged();

                return;
            }
        }

        currentLocation.AddWorldObject(item, playerCellPos);

    }
}
