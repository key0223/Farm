using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] SceneName _sceneNameGoto = SceneName.Scene2_Farm;
    [SerializeField] Vector3 _scenePositionGoto = new Vector3();


    void OnTriggerStay2D(Collider2D collision)
    {
       PlayerController player = collision.GetComponent<PlayerController>();

        if(player != null)
        {
            float x = Mathf.Approximately(_scenePositionGoto.x, 0) ? player.transform.position.x : _scenePositionGoto.x;
            float y = Mathf.Approximately(_scenePositionGoto.y, 0) ? player.transform.position.y : _scenePositionGoto.y;
            float z = 0;

            GameSceneManager.Instance.FadeAndLoadScene(_sceneNameGoto.ToString(),new Vector3(x,y,z));
        }
    }
}
