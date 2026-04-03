using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour, ISaveable
{
    public event Action<int> OnMoneyChanged;

    #region Saveable

    string _isavableUniqueId;
    GameObjectSave _gameObjectSave;

    public string ISaveableUniqueId { get { return _isavableUniqueId; } set { _isavableUniqueId = value; } }
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    #endregion
    PlayerProfile _playerProfile;
    PlayerMove _playerMove;
    PlayerInventory _playerInven;
    PlayerAnimator _playerAnim;
    PlayerActionHandler _playerActionHandler;
    ItemPickup _itemPickup;

    public PlayerProfile PlayerProfile { get { return _playerProfile; } }
    public PlayerMove PlayerMove { get { return _playerMove; } }
    public PlayerInventory PlayerInven { get { return _playerInven; } }
    public PlayerAnimator PlayerAnim { get { return _playerAnim; } }
    public PlayerActionHandler PlayerActionHandler { get { return _playerActionHandler; } }
    public ItemPickup PlayerItemPickup { get { return _itemPickup; } }

    bool _isFirstLoad = true;
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

    public int Money
    {
        get { return _playerProfile.Money; }
        set
        {
            OnMoneyChanged?.Invoke(value);
            _playerProfile.Money = value;
        }
    }
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
        if (!GameManager.Instance.AllManagersReady)
            return;

        UIManager.Instance.OnUIOpenedChanged -= OnUIOpenChanged;
        UIManager.Instance.OnUIOpenedChanged += OnUIOpenChanged;

        ISaveableRegister();
    }
    void OnDisable()
    {
        UIManager.Instance.OnUIOpenedChanged -= OnUIOpenChanged;
        ISaveableDeregister();
    }
    void SubscribeEvent()
    {
        ISaveableRegister();
        UIManager.Instance.OnUIOpenedChanged += OnUIOpenChanged;
        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void CacheComponents()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerInven = GetComponent<PlayerInventory>();
        _playerAnim = GetComponent<PlayerAnimator>();
        _playerActionHandler = GetComponent<PlayerActionHandler>();
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

    void OnUIOpenChanged(bool open)
    {
        CanMove = !open;
    }

    public void SetPlayerProfile(PlayerProfile profile)
    {
        _playerProfile = profile;
        Money=profile.Money;
        if (_isFirstLoad)
        {
            _playerAnim.AnimatedSprite.InitAnimationsFromTableData();
            _isFirstLoad = false;
        }
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
        GameObjectSave.SceneData.Remove(PERSISTENT_SCENE);

        SceneSave sceneSave = new SceneSave();
        sceneSave.Vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.IntDictionary = new Dictionary<string, int>();
        sceneSave.StringDictionary = new Dictionary<string, string>();

        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        sceneSave.Vector3Dictionary.Add("playerPosition", vector3Serializable);
        sceneSave.IntDictionary.Add("playerDirection", _playerMove.CurrentDirection);
        sceneSave.IntDictionary.Add("money", _playerProfile.Money);

        /* Player Profile */
        sceneSave.StringDictionary.Add("farmName", _playerProfile.FarmName);
        sceneSave.StringDictionary.Add("playerName", _playerProfile.PlayerName);
        sceneSave.StringDictionary.Add("hairName", _playerProfile.HairName);
        sceneSave.StringDictionary.Add("hairColor", Parser.ToHexRGBA(_playerProfile.HairColor));


        #region Inventory

        sceneSave.InventoryItemArray = new InventoryItem[_playerInven.PlayerContainer.Storage.Slots.Length];

        for (int i = 0; i < sceneSave.InventoryItemArray.Length; i++)
        {
            Item item = _playerInven.PlayerContainer.Storage.Slots[i];
            if (item == null) continue;

            InventoryItem inventoryItem = new InventoryItem();
            inventoryItem.ItemId = item.Id;
            inventoryItem.ItemQuantity = item.Stack;

            sceneSave.InventoryItemArray[i] = inventoryItem;
        }

        #endregion

        GameObjectSave.SceneData.Add(PERSISTENT_SCENE, sceneSave);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameObjectData.TryGetValue(ISaveableUniqueId, out GameObjectSave gameObjSave))
        {
            GameObjectSave = gameObjSave;
            if (GameObjectSave.SceneData.TryGetValue(PERSISTENT_SCENE, out SceneSave sceneSave))
            {
                PlayerProfile profile = new PlayerProfile();

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
                    if (sceneSave.IntDictionary.TryGetValue("money", out int money))
                    {
                        profile.Money = money;
                    }
                }

                #region Player Profile
                if (sceneSave.StringDictionary != null)
                {
                    if (sceneSave.StringDictionary.TryGetValue("farmName", out string farmName))
                    {
                        profile.FarmName = farmName;
                    }
                    if (sceneSave.StringDictionary.TryGetValue("playerName", out string playerName))
                    {
                        profile.PlayerName = playerName;
                    }
                    if (sceneSave.StringDictionary.TryGetValue("hairName", out string hairName))
                    {
                        profile.HairName = hairName;
                    }
                    if (sceneSave.StringDictionary.TryGetValue("hairColor", out string hairColor))
                    {
                        profile.HairColor = Parser.ParseColor(hairColor);
                    }

                    SetPlayerProfile(profile);

                }
                #endregion

                #region Inventory

                if (sceneSave.InventoryItemArray != null)
                {
                    for (int i = 0; i < sceneSave.InventoryItemArray.Length; i++)
                    {
                        InventoryItem inventoryItem = sceneSave.InventoryItemArray[i];

                        if (inventoryItem.ItemId == 0) continue;

                        Item item = ItemFactory.Create(inventoryItem.ItemId, inventoryItem.ItemQuantity);
                        _playerInven.TryAddAt(i, item);
                    }

                }

                #endregion

            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //
    }

    #endregion
}
