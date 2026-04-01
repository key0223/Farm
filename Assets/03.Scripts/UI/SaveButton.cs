using TMPro;
using UnityEngine;

public class SaveButton : ClickableComponent
{
    [SerializeField] bool _isYesButton;
    [SerializeField] TextMeshProUGUI _optionText;
    [SerializeField] Color _defaultColor;
    [SerializeField] Color _highlightColor;

     bool _isHovered = false;
     public override void OnHover()
    {
        if (_isHovered) return;

        _isHovered = true;
        _optionText.color = _highlightColor;
    }
    public override void OnHoverExit()
    {
        if(!_isHovered) return;

        _isHovered = false;
        _optionText.color = _defaultColor;
    }

    public override void OnLeftClick(Vector2 pos)
    {
        if(_isYesButton)
        {
            TimeManager.Instance.SleepToMorning();
            SaveLoadManager.Instance.SaveDataToFile();
            UIManager.Instance.HideSave();

            GameSceneManager.Instance.Fade();

        }
        else
        {
            UIManager.Instance.HideSave();
        }
    }
}
