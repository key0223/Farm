using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    readonly Dictionary<Keys, (string name, Type type)> _uiButtons = new()
    {
        {Keys.E,("Inventory",typeof(InventoryMenu))},

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

    Dictionary<string, ClickableMenu> _menuCache = new Dictionary<string, ClickableMenu>();

    Stack<ClickableMenu> _menuStack = new Stack<ClickableMenu>(); /* 이전 메뉴들 저장 */
    List<ClickableMenu> _pages = new List<ClickableMenu>(); /* 탭,페이지 전환용 */

    ToolbarMenu _toolbar;
    ClickableMenu _activeMenu;

    TooltipUI _tooltip;

    public Dictionary<string, ClickableMenu> MenuCache { get { return _menuCache; } }
    public ClickableMenu ActiveMenu { get { return _activeMenu; } set { _activeMenu = value; } }
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAllManagersReady += SubscribeEvent;

        CacheAllMenus();
        _toolbar = FindObjectOfType<ToolbarMenu>();
        _tooltip = FindObjectOfType<TooltipUI>();
        GameManager.Instance.ManagerReady("UIManager");

    }

    void OnEnable()
    {
        if (!GameManager.Instance.AllMamagersReady)
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
    void HandleEscape()
    {
        if (_activeMenu != null && _activeMenu.ShouldExitOnEscapeKey())
            PopMenu();
    }
    void ToggleMenu(string menuName, Type menuType)
    {
        HideTooltip();
        ClickableMenu active = _activeMenu;

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
            _activeMenu.gameObject.SetActive(true);
            _activeMenu.PopulateClickableComponentList();
            HideToolbar();
        }
        else
            ShowToolbar();
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
    public void ShowTooltip(string name, string itemType,string color,string description, Vector2 mousePos)
    {
        _tooltip.Show(name, itemType,color, description, mousePos);
    }
    public void HideTooltip()
    {
        _tooltip.Hide();
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

        // 토글 로직
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
