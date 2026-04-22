using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    PlayerController _player;

    [Header("Shop Info UI References")]
    [SerializeField] Image _shopIconImage;
    [SerializeField] TextMeshProUGUI _shopNameText;
    [SerializeField] TextMeshProUGUI _shopItemTabText;
    [SerializeField] TextMeshProUGUI _myItemTabText;
    [Header("Item Info UI References")]
    [SerializeField] GameObject _itemInfoObj;
    [SerializeField] TextMeshProUGUI _itemNameText;
    [SerializeField] TextMeshProUGUI _itemCategoryText;
    
    string _currentShopId;
    ObjectItem _selectedItem;


    public PlayerController Player {  get { return _player; } }
    public string CurrentShopId { get { return _currentShopId; } }
   
    public void SetCurrentShop(string shopId,PlayerController player)
    {
        if(_currentShopId == shopId) return;
        _currentShopId = shopId;
        _player = player;
        RefreshMenu();
    }
    public void SetSelectedItem(ObjectItem item)
    {
        if(_selectedItem == item|| item==null)
        {
            _selectedItem = null;
            _itemInfoObj.gameObject.SetActive(false);
        }
        else
        {
            _selectedItem = item;
            _itemNameText.text = LocalizationManager.Instance.GetString(item.DisplayName);
            _itemCategoryText.text = LocalizationManager.Instance.GetString(item.Category);
            string color = item.CategoryColor;
            _itemCategoryText.color = Parser.ParseColor(color);

            _itemInfoObj.gameObject.SetActive(true);
        }
    }
    void RefreshMenu()
    {
        if(_currentShopId == null) return;

        SetSelectedItem(null);

        ShopDataBase data;
        TableDataManager.Instance.ShopDict.TryGetValue(_currentShopId, out data);

        _shopNameText.text = LocalizationManager.Instance.GetString(data.DisplayName);
        _shopItemTabText.text = LocalizationManager.Instance.GetString("ShopItems");
        _myItemTabText.text = LocalizationManager.Instance.GetString("MyItems");
    }
}
