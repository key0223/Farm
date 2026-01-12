using System;
using System.Collections;
[Serializable]

public class SceneItem  /* 씬에 배치(드랍)된 아이템 저장 */
{
    public int ItemId;
    public Vector3Serializable Position;

    public SceneItem()
    {
        Position = new Vector3Serializable();
    }
}
