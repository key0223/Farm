using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponent : MonoBehaviour
{
    [SerializeField] int _id;
    Rect _bounds;

    public int ClickableId { get { return _id; } set { _id = value; } }
    void Start()
    {
        _bounds = GetComponent<RectTransform>().rect;
    }

    public virtual bool ContainsPoint(int x, int y)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            new Vector2(x,y), null, out localPoint);

        bool contains = _bounds.Contains(localPoint);
        return contains;
    }
    
    public virtual void OnHover() { }
    public virtual void OnLeftClick(Vector2 pos) { }
}
