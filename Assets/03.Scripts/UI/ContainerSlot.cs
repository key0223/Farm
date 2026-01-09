using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContainerSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IEndDragHandler,
    IDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] int _slotIndex = 0;
    [SerializeField] GameObject _dragItemPrefab;

    [Header("UI References")]
    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _stackText;

    InputState _input;
    Container _ownerContainer;

    Canvas _dragCanvas;
    CanvasGroup _canvasGroup;

    GameObject _dragItemObj;
    Item _currentItem;
    Item _draggedItem;
    int _originalStack;

    public int SlotIndex { get { return _slotIndex; } set { _slotIndex = value; } }
    public Container OwnerContainer { get { return _ownerContainer; } set { _ownerContainer = value; } }
    public Item Currentitem { get { return _currentItem; } }
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _input = InputManager.Instance.InputState;
        UpdateVisual();
    }
    public void SetItem(Item item)
    {
        _currentItem = item;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if(_currentItem!= null)
        {
            _iconImage.sprite = _currentItem.Icon;
            _stackText.text = _currentItem.Stack > 1 ? _currentItem.Stack.ToString() : "";

            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.enabled = false;
            _stackText.text = "";
        }
    }
  
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideTooltip();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_input.IsNewLeftClick() && _currentItem != null)
        {
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
            Debug.Log($"[Slot {_slotIndex}] Left Down: {_currentItem.Name}");
        }

        if (_input.IsNewRightClick() && _currentItem != null && _currentItem.Stackable)
        {
            SplitStack();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_input.IsLeftHeld() || _currentItem == null)
        {
            /* 드래그 취소 */
            eventData.pointerDrag = null;
            return;
        }
        _iconImage.enabled = false;
        _draggedItem = _currentItem.Clone();
        _originalStack = _draggedItem.Stack;
        _ownerContainer.Storage.SetItemAtSlot(_slotIndex, null);

        GlobalDragManager.Instance.StartDrag(_draggedItem, _ownerContainer,_slotIndex);
        CreateDragItem(_draggedItem);
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_input.IsLeftHeld() && _dragItemObj != null)
            _dragItemObj.transform.position = _input.MousePosition;

        /* 스크롤 수량 조절 */
        if(Mathf.Abs(_input.ScrollDelta) > 0.01f)
        {
            int dir = _input.ScrollDelta >0 ? 1 : -1;
            AdjustDragStack(dir);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_ownerContainer == null || _currentItem == null)
        {
            Debug.LogError($"ContainerSlot {_slotIndex}: container={_ownerContainer}, currentItem={_currentItem}", this);
            CleanupDrag();
            RefreshSlot();
            return;
        }

        Container targetContainer = GetTargetContainer(eventData);
        int targetSlot = GetTargetSlot(eventData);

        GlobalDragManager.Instance.EndDrag(
            sourceContainer: _ownerContainer,
            sourceSlot: _slotIndex,
            targetContainer: targetContainer,
            targetSlot: targetSlot
            );

        CleanupDrag();
        RefreshSlot();
    }

    Container GetTargetContainer(PointerEventData eventData)
    {
        if(eventData == null) return null;

        if (eventData.pointerEnter != null)
        {
            ContainerSlot targetSlot = eventData.pointerEnter.GetComponentInParent<ContainerSlot>();
            if (targetSlot != null)
                return targetSlot._ownerContainer;
        }

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData,results);

        foreach (var result in results)
        {
            var slot = result.gameObject.GetComponentInParent<ContainerSlot>();
            if (slot != null && slot != this && slot._ownerContainer != null)
            {
                return slot._ownerContainer;
            }
        }
        return null;
    }
    int GetTargetSlot(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var slot = result.gameObject.GetComponentInParent<ContainerSlot>();
            if (slot != null && slot != this)
            {
                return slot._slotIndex;
            }
        }
        return -1;
    }
    void SplitStack()
    {
        int splitAmount = Mathf.Max(1, _currentItem.Stack / 2);
        Item splitItem = _currentItem.Clone();
        splitItem.Stack = splitAmount;
        _currentItem.Stack -= splitAmount;

        _ownerContainer.TryAdd(splitItem);
        RefreshSlot();
    }
    void AdjustDragStack(int direction)
    {
        if (_draggedItem == null || !_draggedItem.Stackable) return;

        int newStack = _draggedItem.Stack + direction;
        newStack = Mathf.Clamp(newStack, 1, _originalStack);  // 원래 스택 초과 불가

        _draggedItem.Stack = newStack;
        UpdateDragStackText(newStack);

    }

    void CreateDragItem(Item item)
    {
        _dragCanvas = FindObjectOfType<Canvas>();
        _dragItemObj = Instantiate(_dragItemPrefab);
        _dragItemObj.transform.SetParent(_dragCanvas.transform, false);

        Image image = _dragItemObj.GetComponentInChildren<Image>();
        image.sprite = item.Icon;
        image.raycastTarget = false;

        TextMeshProUGUI stackText = _dragItemObj.GetComponentInChildren<TextMeshProUGUI>();
        stackText.text = item.Stack > 1 ? item.Stack.ToString() : "";
    }

    void RefreshSlot()
    {
        _currentItem = _ownerContainer.Storage.GetItemAtSlot(_slotIndex);
        UpdateVisual();
    }
    void UpdateDragStackText(int stackAmount)
    {
        TextMeshProUGUI stackText = _dragItemObj.GetComponentInChildren<TextMeshProUGUI>();
        if (stackText != null)
        {
            stackText.text = stackAmount > 1 ? stackAmount.ToString() : "";
        }
    }
    void CleanupDrag()
    {
        if(_dragItemObj)
            Destroy(_dragItemObj);
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
}
