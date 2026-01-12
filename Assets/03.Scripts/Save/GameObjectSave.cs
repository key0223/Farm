using System;
using System.Collections.Generic;

[Serializable]
public class GameObjectSave /* 게임 오브젝트 단위 전체 저장 데이터 */
{
    // key = scene name ,  value = 씬에서의 오브젝트 상태
    public Dictionary<string, SceneSave> SceneData;

    public GameObjectSave()
    {
        SceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.SceneData = sceneData;
    }
}
