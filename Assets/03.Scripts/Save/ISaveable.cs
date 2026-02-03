using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable 
{
    string ISaveableUniqueId { get; set;}
    GameObjectSave GameObjectSave { get; set; } /* 오브젝트에 대한 저장 데이터 전체 */

    void ISaveableRegister();

    void ISaveableDeregister();

    GameObjectSave ISaveableSave(); /* 현재 상태를 GameObjectSave 형태로 만들어 반환 */

    void ISaveableLoad(GameSave gameSave);


    /* 씬 별 데이터를 저장,복구 */
    void ISaveableStoreScene(string sceneName);

    void ISaveableRestoreScene(string sceneName);
}
