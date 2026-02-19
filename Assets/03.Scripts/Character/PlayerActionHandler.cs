using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerActionHandler : MonoBehaviour
{
    PlayerController _playerController;
    InputState _input;
    float _maxToolRange = 2f;
    float _maxInteractRange = 1.5f;

    readonly List<RaycastResult> _rayResults = new List<RaycastResult>(10);
    LayerMask _interactableMask = -1;
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _input = InputManager.Instance.InputState;
        _interactableMask = LayerMask.GetMask("Interactable", "NPC");
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (_playerController.PlayerAnim.IsInAction) return;

        if (_input.IsNewLeftClick())
        {
            int selectedIndex = _playerController.PlayerInven.CurrentToolbarIndex;
            Item selected = _playerController.PlayerInven.PlayerContainer.Storage.GetItemAtSlot(selectedIndex);
            Vector2 mousePos = _input.MousePosition;
            if (selected == null)
            {
                HandleLeftClick(mousePos);
                return;
            }

            HandleItemUse(selected, mousePos);
        }

        if (_input.IsNewRightClick())
        {
            Vector2 mousePos = _input.MousePosition;
            HandleRightClick(mousePos);
        }
    }

    void HandleLeftClick(Vector2 mousePos)
    {
        if (IsPointerOverUI(mousePos)) return;

        Vector3Int playerCellPos = GetPlayerCellPos();
        Vector3Int targetCell = GetValidTargetCell(mousePos);
        int targetDirection = _playerController.GetDirectionToMouse(mousePos);
        _playerController.CurrentDirection = targetDirection;

        if (IsInToolRange(playerCellPos, targetCell))
        {
            GameLocation location = MapManager.Instance.CurrentLocation;
            HoeDirtFeature dirt = location.GetRuntimeFeature(targetCell.x, targetCell.y) as HoeDirtFeature;

            if (dirt != null && dirt.CurrentCrop != null && dirt.CurrentCrop.FullyGrown)
            {
                dirt.Harvest();
                SoundManager.Instance.PlaySound(SoundName.EFFECT_PLUCK);
            }
        }
    }

    void HandleRightClick(Vector2 mousePos)
    {
        if (IsPointerOverUI(mousePos)) return;

        Vector3Int playerCellPos = GetPlayerCellPos();
        Vector3Int targetCell = GetValidTargetCell(mousePos);

        if (!IsInInteractRange(playerCellPos, targetCell)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -10f));
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0.1f, _interactableMask);

        if(hit.collider !=null)
        {
            int selectedIndex = _playerController.PlayerInven.CurrentToolbarIndex;
            Item selected = _playerController.PlayerInven.PlayerContainer.Storage.GetItemAtSlot(selectedIndex);

            MapManager.Instance.CurrentLocation.HandleRaycastHit(_playerController, hit, selected);
        }
    }

   
    void HandleItemUse(Item item, Vector2 mousePos)
    {
        if (IsPointerOverUI(mousePos)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -10f));
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0.1f, LayerMask.GetMask("Interactable"));

        if (hit.collider != null)
        {
            // TODO : NPC 또는 상호작용 가능한 오브젝트 액션 처리
            return;
        }

        Vector3Int playerCellPos = GetPlayerCellPos();
        Vector3Int targetCell = GetValidTargetCell(mousePos);
        int targetDirection = _playerController.GetDirectionToMouse(mousePos);
        _playerController.CurrentDirection = targetDirection;

        if (item is Tool tool)
        {
            _playerController.CanMove = false;
            targetCell = GetToolTargetCell(playerCellPos, targetCell, targetDirection);
            HandleToolUse(tool, targetCell);

        }
        else if (item is ObjectItem obj && obj.ObjectType == ObjectType.SEEDS)
        {

            targetCell = GetValidTargetCell(mousePos);
            if (IsInToolRange(playerCellPos, targetCell))
                PlantSeed(obj, targetCell);
        }
    }

    void HandleToolUse(Tool tool, Vector3Int targetCell)
    {
        switch (tool)
        {
            case Shovel shovel:
                shovel.UseTool(targetCell);
                _playerController.PlayerAnim.PlayAction("dig", _playerController.CurrentDirection);
                break;
            case WateringCan watering:
                watering.UseTool(targetCell);
                _playerController.PlayerAnim.PlayAction("watering", _playerController.CurrentDirection);
                SoundManager.Instance.PlaySound(SoundName.EFFECT_WATERING_CAN);
                break;
        }
    }

    void PlantSeed(ObjectItem seed, Vector3Int targetCell)
    {
        GameLocation location = MapManager.Instance.CurrentLocation;
        HoeDirtFeature dirt = location.GetRuntimeFeature(targetCell.x, targetCell.y) as HoeDirtFeature;

        if (dirt != null && dirt.CurrentCrop == null)
        {
            dirt.Plant(location, seed.Id);
            _playerController.PlayerInven.TryRemove(seed.Id, 1);

            SoundManager.Instance.PlaySound(SoundName.EFFECT_PLANTING);
        }

    }
    bool IsPointerOverUI(Vector2 mousePos)
    {
        _rayResults.Clear();

        PointerEventData poinerData = new PointerEventData(EventSystem.current)
        {
            position = mousePos,
        };
        EventSystem.current.RaycastAll(poinerData, _rayResults);

        return _rayResults.Count > 0;
    }


    Vector3Int GetToolTargetCell(Vector3Int playerCellPos, Vector3Int mouseCell, int direction)
    {
        if (IsInToolRange(playerCellPos, mouseCell))
            return mouseCell;

        return GetFrontTile(playerCellPos, direction);
    }
    bool IsInToolRange(Vector3Int playerCell, Vector3Int targetCell)
    {
        float tileDistance = Vector3.Distance(playerCell, targetCell);

        return tileDistance <= _maxToolRange;
    }
    bool IsInInteractRange(Vector3Int playerCellPos, Vector3Int targetCell)
    {
        float tileDistance = Vector3.Distance(playerCellPos, targetCell);
        return tileDistance <= _maxInteractRange;
    }
    Vector3Int GetPlayerCellPos()
    {
        Vector3Int pos = _playerController.CellPos;

        return pos;
    }
    Vector3Int GetValidTargetCell(Vector2 mousePos)
    {
        Vector3Int mouseCell = GridUtils.ScreenToGridPos(mousePos);

        return mouseCell;
    }

    Vector3Int GetFrontTile(Vector3Int playerCellPos, int direction)
    {
        Vector3Int front = playerCellPos;
        switch (direction)
        {
            case 0: front.x -= 1; break;  // Left
            case 1: front.x += 1; break;  // Right
            case 2: front.y += 1; break;  // Up  
            case 3: front.y -= 1; break;  // Down
        }
        return front;
    }
}
