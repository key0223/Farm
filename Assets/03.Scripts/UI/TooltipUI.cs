using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _itemNameText;
    [SerializeField] TextMeshProUGUI _itemTypeText;
    [SerializeField] TextMeshProUGUI _itemDescriptionText;

    Canvas _overlayCanvas;
    RectTransform _rectTransform;

    void Awake()
    {
        _overlayCanvas = GameObject.Find("Overlay Layer").GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string name, string itemType,string color, string description, Vector2 mousePos)
    {
        _itemNameText.text = name;
        _itemTypeText.text = itemType;
        _itemTypeText.color = Parser.ParseColor(color);
        _itemDescriptionText.text = description;

        _rectTransform.pivot = new Vector2(0f, 0f);
        Vector2 pos = mousePos + new Vector2(20f, 10f);

        pos.x = Mathf.Clamp(pos.x, 10, Screen.width - 250);
        pos.y = Mathf.Clamp(pos.y, 10, Screen.height - 100);

        transform.position = pos;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
