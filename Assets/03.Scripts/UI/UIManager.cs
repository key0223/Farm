using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static Define;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    public event Action<bool> OnUIOpenedChanged;

    readonly Dictionary<Keys, (string name, Type type)> _uiButtons = new()
    {
        {Keys.E,("Inventory",typeof(InventoryMenu))},
        //{Keys.I,("CookingCollection",typeof(CookingCollectionMenu))},

        /* Toolbar */
        {Keys.Alpha1,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha2,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha3,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha4,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha5,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha6,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha7,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha8,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha9,("Toolbar",typeof(ToolbarMenu))},
        {Keys.Alpha0,("Toolbar",typeof(ToolbarMenu))},

    };

    Dictionary<string, string> _tabContainers = new()
    {
        {"Inventory","GameMenuContainer" },
        {"CookingCollection","GameMenuContainer" },
        {"Buy","ShopContainer" },
        {"Sell","ShopContainer" },
    };
    Dictionary<string, ClickableMenu> _menuCache = new Dictionary<string, ClickableMenu>();

    Stack<ClickableMenu> _menuStack = new Stack<ClickableMenu>(); /* ���� �޴��� ���� */

    Dictionary<string,List<ClickableMenu>> _tabGroups = new Dictionary<string, List<ClickableMenu>>();
    Dictionary<string, int> _currentTabIndices = new Dictionary<string, int>();

    ClickableMenu _activeMenu;
    ToolbarMenu _toolbar;

    TooltipUI _tooltip;
    DialogueUI _dialogueUI;
    SimpleMessageUI _simpleMessageUI;
    CharacterCustomizationUI _customizeUI;
    ShopMenu _shopMenu;
    SaveUI _saveUI;


    public Dictionary<string, ClickableMenu> MenuCache { get { return _menuCache; } }
    public ClickableMenu ActiveMenu { get { return _activeMenu; } set { _activeMenu = value; } }
    public DialogueUI DialogueUI { get { return _dialogueUI; } }
    public SimpleMessageUI SimpleMessageUI { get { return _simpleMessageUI; } }
    public CharacterCustomizationUI CharacterCustomizationUI { get { return _customizeUI; } }
    public ShopMenu ShopUI { get { return _shopMenu; } }
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        CacheAllMenus();
        _toolbar = FindObjectOfType<ToolbarMenu>();
        _tooltip = FindObjectOfType<TooltipUI>();
        _dialogueUI = FindObjectOfType<DialogueUI>();
        _simpleMessageUI = FindObjectOfType<SimpleMessageUI>();
        _customizeUI = FindObjectOfType<CharacterCustomizationUI>();
        _shopMenu = FindObjectOfType<ShopMenu>();
        _saveUI = FindObjectOfType<SaveUI>();
        GameManager.Instance.ManagerReady("UIManager");

    }

    void OnEnable()
    {
        if (!GameManager.Instance.AllManagersReady)
            return;

        InputManager.Instance.OnKeyPressed -= HandleKeyPressed;
        InputManager.Instance.OnEscapePressed -= HandleEscape;

        InputManager.Instance.OnKeyPressed += HandleKeyPressed;
        InputManager.Instance.OnEscapePressed += HandleEscape;
    }
    void OnDisable()
    {
        InputManager.Instance.OnKeyPressed -= HandleKeyPressed;
        InputManager.Instance.OnEscapePressed -= HandleEscape;
    }

    void SubscribeEvent()
    {
        InputManager.Instance.OnKeyPressed += HandleKeyPressed;
        InputManager.Instance.OnEscapePressed += HandleEscape;

        GameManager.OnAllManagersReady -= SubscribeEvent;
    }
    void CacheAllMenus()
    {
        ClickableMenu[] allMenus = FindObjectsByType<ClickableMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        _menuCache.Clear();

        foreach (ClickableMenu menu in allMenus)
        {
            if (!menu.gameObject.scene.IsValid())
                continue;

            if (!string.IsNullOrEmpty(menu.MenuName) && !_menuCache.ContainsKey(menu.MenuName))
                _menuCache.Add(menu.MenuName, menu);
        }
    }

    public void RegisterTabs(string containerName, List<ClickableMenu> tabs)
    {
        _tabGroups[containerName] = tabs;
        _currentTabIndices[containerName] = 0;
        //ShowTab(containerName, 0);
    }
    public ClickableMenu GetActiveTab(string containerName)
    {
        if(_tabGroups.TryGetValue(containerName, out var tabs) &&
           _currentTabIndices.TryGetValue(containerName,out int index))
            return tabs[index];
        return null;
    }
    public List<ClickableMenu> GetTabs(string containerName)
    {
        if(_tabGroups.TryGetValue(containerName,out var tabs))
            return tabs;
        return null;
    }

    public int GetCurrentTabIndex(string containerName)
    {
        _currentTabIndices.TryGetValue(containerName, out int index);
        return index;
    }
    void HandleKeyPressed(Keys key)
    {
        if (_uiButtons.TryGetValue(key, out var menuInfo))
        {
            if (menuInfo.name == "Toolbar")
            {
                ToolbarMenu toolbar = FindObjectOfType<ToolbarMenu>();
                if (toolbar != null)
                {
                    int slotIndex = GetHotbarIndexFromKey(key);
                    toolbar.SelectToolbarSlot(slotIndex);
                }
            }
            else
            {
                ToggleMenu(menuInfo.name, menuInfo.type);

            }
        }

    }
    public void HandleEscape()
    {
        if (_activeMenu != null && _activeMenu.ShouldExitOnEscapeKey())
        {

            PopMenu();
            HideTooltip();
        }

    }
    void ToggleMenu(string menuName, Type menuType)
    {
        HideTooltip();
        ClickableMenu active = _activeMenu;

        // �� �޴� Ȯ��
        string containerName = GetContainerForTab(menuName);
        if (!string.IsNullOrEmpty(containerName))
        {
            ClickableMenu container  = FindMenuByName(containerName);

            /* 컨테이너 활성, 클릭한 탭 활성 -> 컨테이너 닫기 */
            if(_activeMenu == container)
            {
                int currentIndex = GetCurrentTabIndex(containerName);
                int targetIndex = GetTabIndex(containerName,menuName);

                if(currentIndex == targetIndex)
                {
                    PopMenu();
                    return;
                }
            }

            /* 탭 전환 */
            if (container != null)
            {
                if(_activeMenu != container)
                    OpenMenuByName(container.MenuName);

                int tabIndex = GetTabIndex(containerName, menuName);
                ShowTab(containerName, tabIndex);
                return;
            }
        }

        /* 일반 메뉴 토글 */
        if (active != null && menuType.IsInstanceOfType(active))
        {
            PopMenu();
            return;
        }

        if (active != null)
            return;

        OpenMenuByName(menuName);

    }
    void PushMenu(ClickableMenu menu)
    {
        if (_activeMenu != null)
            _menuStack.Push(_activeMenu);
        SetActiveMenu(menu);
    }

    void PopMenu()
    {
        if (_menuStack.Count > 0)
            SetActiveMenu(_menuStack.Pop());
        else
            SetActiveMenu(null);

    }

    void SetActiveMenu(ClickableMenu menu)
    {
        if (_activeMenu != null)
            _activeMenu.gameObject.SetActive(false);

        _activeMenu = menu;

        if (_activeMenu != null)
        {
            if (menu.MenuName != "Title")
                SoundManager.Instance.PlaySound(SoundName.UI_OPEN);

            _activeMenu.gameObject.SetActive(true);
            _activeMenu.PopulateClickableComponentList();
            HideToolbar();
            OnUIOpenedChanged?.Invoke(true);
        }
        else
        {
            if (menu == null ||  menu.MenuName != "Title")
                SoundManager.Instance.PlaySound(SoundName.UI_CLOSE);

            ShowToolbar();
            OnUIOpenedChanged?.Invoke(false);
        }
    }

    public void ShowTab(string containerName, int index)
    {
        if (!_tabGroups.TryGetValue(containerName, out var tabs) || index < 0 || index >= tabs.Count) return;

        
        for (int i = 0; i < tabs.Count; i++)
        {
            bool isActive = (i== index);
            tabs[i].gameObject.SetActive(isActive);
        }

        _currentTabIndices[containerName] = index;

        if (_activeMenu != null)
            _activeMenu.PopulateClickableComponentList();
    }
   
    #region Toolbar
    void HideToolbar()
    {
        if (_toolbar != null && _toolbar.gameObject.activeSelf)
            _toolbar.gameObject.SetActive(false);
    }

    void ShowToolbar()
    {
        if (_toolbar != null && !_toolbar.gameObject.activeSelf)
        {
            _toolbar.gameObject.SetActive(true);
            _toolbar.UpdateHighlightVisual();
        }
    }
    #endregion

    #region Tooltip
    public void ShowTooltip(string name, string itemType, string color, string description, Vector2 mousePos)
    {
        _tooltip.Show(name, itemType, color, description, mousePos);
    }
    public void HideTooltip()
    {
        _tooltip.Hide();
    }
    #endregion

    #region Dialogue

    public void ShowDialogue()
    {
        OpenMenuByName("Dialogue");
    }
    public void HideDialogue()
    {
        OpenMenuByName("Dialogue");
    }

    #endregion

    #region Simple Message
    public void ShowSimpleMessage(string message)
    {
        _simpleMessageUI.Show(message);
        OpenMenuByName("SimpleMessage");
    }
    public void HideSimpleMessage()
    {
        OpenMenuByName("SimpleMessage");
    }

    #endregion

    #region Title
    public void ShowTitle()
    {
        OpenMenuByName("Title");
    }
    public void HideTitle()
    {
        OpenMenuByName("Title");
    }
    #endregion
    #region Shop
    public void ShowShop(string shopId, PlayerController player)
    {
        _shopMenu.SetCurrentShop(shopId, player);
        ClickableMenu container = FindMenuByName("ShopContainer");
        if (container == null) return;

        if (_activeMenu != container)
        {
            PushMenu(container);

            int buyTabIndex = GetTabIndex("ShopContainer", "Buy");  // 0
            ShowTab("ShopContainer", buyTabIndex);
            return;
        }
        if (container == _activeMenu)
        {
            int buyTabIndex = GetTabIndex("ShopContainer", "Buy");
            ShowTab("ShopContainer", buyTabIndex);
        }
    }
    public void HideShop()
    {
        OpenMenuByName("ShopContainer");
    }
    #endregion

#region Save UI
public void ShowSave()
    {
        OpenMenuByName("Save");
    }
    public void HideSave()
    {
        OpenMenuByName("Save");
    }
#endregion
    #region Helpers

    public ClickableMenu OpenMenuByName(string menuName)
    {
        ClickableMenu menu = FindMenuByName(menuName);
        if (menu == null)
        {
            Debug.LogWarning($"{menuName} : not found.");
            return null;
        }

        // ��� ����
        if (_activeMenu == menu)
        {
            PopMenu();
            return null;
        }
        if (_activeMenu != null)
        {
            Debug.Log($"Ignored {menuName}: {_activeMenu.MenuName} active");
            return null;
        }

        PushMenu(menu);
        return menu;
    }
    ClickableMenu FindMenuByName(string menuName)
    {
        if (string.IsNullOrEmpty(menuName)) return null;
        _menuCache.TryGetValue(menuName, out ClickableMenu menu);
        return menu;
    }
    public T OpenMenuByName<T>(string menuName) where T : ClickableMenu
    {
        T menu = FindMenuByName<T>(menuName);
        if (menu != null)
        {
            PushMenu(menu);
            return menu;
        }

        Debug.LogWarning($"{menuName} : not found.");
        return null;
    }
    public T OpenMenuByID<T>(int menuId) where T : ClickableMenu
    {
        T[] allMenus = FindObjectsOfType<T>();

        foreach (T menu in allMenus)
        {
            if (menu.MenuId == menuId)
            {
                PushMenu(menu);
                return menu;
            }
        }

        return null;
    }

    T FindMenuByName<T>(string menuName) where T : ClickableMenu
    {
        if (string.IsNullOrEmpty(menuName)) return null;

        if (_menuCache.TryGetValue(menuName, out ClickableMenu menu))
            return menu as T;

        return null;
    }

    string GetContainerForTab(string tabName)
    {
        return _tabContainers.TryGetValue(tabName, out var container) ? container : null;
    }
    int GetTabIndex(string container, string tabName)
    {
        return (container,tabName) switch
        {
            ("GameMenuContainer", "Inventory")=>0,
            ("GameMenuContainer", "CookingCollection") =>1,
            ("ShopContainer", "Buy") =>0,
            ("ShopContainer", "Sell") =>1,
            _ => 0
        };
    }
    int GetHotbarIndexFromKey(Keys key)
    {
        return key switch
        {
            Keys.Alpha1 => 0,
            Keys.Alpha2 => 1,
            Keys.Alpha3 => 2,
            Keys.Alpha4 => 3,
            Keys.Alpha5 => 4,
            Keys.Alpha6 => 5,
            Keys.Alpha7 => 6,
            Keys.Alpha8 => 7,
            Keys.Alpha9 => 8,
            Keys.Alpha0 => 9,
            _ => 0
        };
    }
    #endregion
}
