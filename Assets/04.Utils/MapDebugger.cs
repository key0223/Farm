using UnityEngine;

public class MapDebugger : MonoBehaviour
{
    [Header("Settings")]
    public bool showGizmos = true;
    public Color areaColor = new Color(0, 1, 0, 0.3f); // 연한 녹색
    public Color outlineColor = Color.green;

    public int checkX;
    public int checkY;
    public string targetMapDataName = "";
    public MapData _targetMapData;

    [ContextMenu("SetMapData")]
    public void SetMapData()
    {
         GameLocation location =  MapManager.Instance.GetLocation(targetMapDataName);
        _targetMapData = location.MapData;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || _targetMapData == null) return;

        // 실제 타일 영역(Actual Bounds) 계산
        Vector3 bottomLeft = GridUtils.GridToWorld(new Vector3Int(_targetMapData._minX, _targetMapData._minY, 0));

        float width = _targetMapData._mapWidth;
        float height = _targetMapData._mapHeight;

        Vector3 center = bottomLeft + new Vector3(width / 2f, height / 2f, 0);
        Vector3 size = new Vector3(width, height, 0.1f);

        Gizmos.color = areaColor;
        Gizmos.DrawCube(center, size);

        Gizmos.color = outlineColor;
        Gizmos.DrawWireCube(center, size);

        // 시작점(MinX, MinY)에 작은 구체 표시
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bottomLeft, 0.2f);

        Vector3 targetWorldPos = GridUtils.GridToWorld(new Vector3Int(checkX, checkY, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetWorldPos, 0.5f);
    }
}