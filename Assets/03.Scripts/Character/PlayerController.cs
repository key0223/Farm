using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour, ISaveable
{
    #region Saveable

    string _isavableUniqueId;
    GameObjectSave _gameObjectSave;

    public string ISaveableUniqueId { get { return _isavableUniqueId; } set { _isavableUniqueId = value; } }
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    #endregion

    PlayerMove _playerMove;
    PlayerInventory _playerInven;
    PlayerAnimator _playerAnim;
    PlayerActionHanlder _playerActionHanlder;
    ItemPickup _itemPickup;

    public PlayerMove PlayerMove { get { return _playerMove; } }
    public PlayerInventory PlayerInven { get { return _playerInven; } }
    public PlayerAnimator PlayerAnim { get { return _playerAnim; } }
    public PlayerActionHanlder PlayerActionHanlder { get { return _playerActionHanlder; } }
    public ItemPickup PlayerItemPickup { get { return _itemPickup; } }

    #region Properties

    public bool CanMove { get { return _playerMove.CanMove; } set { _playerMove.CanMove = value; } }
    public int CurrentDirection
    {
        get { return _playerMove.CurrentDirection; }
        set
        {
            _playerMove.CurrentDirection = value;
            if (value == 0 || value == 1)  // Left/Right
            {
                _playerMove.LastHorizontalDirection = value;
            }
            _playerAnim.AnimatedSprite.SetDirection(value);
        }
    }
    public Vector3Int CellPos { get { return _playerMove.CellPos; } set { _playerMove.CellPos = value; } }
    #endregion

    void Awake()
    {
        CacheComponents();
        GameManager.OnAllManagersReady += SubscribeEvent;

        _isavableUniqueId = GetComponent<GenerateGUID>().GUID;
        _gameObjectSave = new GameObjectSave();
    }
    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
            return;

        ISaveableRegister();
    }
    void OnDisable()
    {
        ISaveableDeregister();
    }
    void SubscribeEvent()
    {
        ISaveableRegister();

        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void CacheComponents()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerInven = GetComponent<PlayerInventory>();
        _playerAnim = GetComponent<PlayerAnimator>();
        _playerActionHanlder = GetComponent<PlayerActionHanlder>();
        _itemPickup = GetComponent<ItemPickup>();
    }

    public int GetDirectionToMouse(Vector2 mousePos)
    {
        Vector3Int playerCell = CellPos;
        Vector3Int mouseCell = GridUtils.ScreenToGridPos(mousePos);

        Vector3Int direction = mouseCell - playerCell;

        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            return direction.y > 0 ? 2 : 3;
        else
            return direction.x > 0 ? 1 : 0;
    }

    [SerializeField] string TestDialogue;

    [ContextMenu("DialogueTest")]
    public void DialogueTest()
    {
        DialogueManager.Instance.StartDialogue("Rand", TestDialogue);
    }
    #region Saveable
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.ISaveableList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.ISaveableList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(PERSISTENT_SCENE);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameObjectData.TryGetValue(ISaveableUniqueId, out GameObjectSave gameObjSave))
        {
            GameObjectSave = gameObjSave;
            ISaveableRestoreScene(PERSISTENT_SCENE);
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.SceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();
        sceneSave.Vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.IntDictionary = new Dictionary<string, int>();

        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        sceneSave.Vector3Dictionary.Add("playerPosition", vector3Serializable);
        sceneSave.IntDictionary.Add("playerDirection", _playerMove.CurrentDirection);

        GameObjectSave.SceneData.Add(sceneName, sceneSave);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.SceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.Vector3Dictionary != null && sceneSave.Vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
            {
                transform.position = new Vector3(playerPosition.X, playerPosition.Y, playerPosition.Z);
            }

            if (sceneSave.IntDictionary != null)
            {
                if (sceneSave.IntDictionary.TryGetValue("playerDirection", out int playerDir))
                {
                    _playerMove.CurrentDirection = playerDir;
                }
            }
        }
    }

    #endregion
}
