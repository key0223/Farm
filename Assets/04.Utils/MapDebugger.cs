using UnityEngine;

public class MapDebugger : MonoBehaviour
{
    [Header("Settings")]
    public bool showGizmos = true;
    public Color areaColor = new Color(0, 1, 0, 0.3f); // 연한 녹색
    public Color outlineColor = Color.green;

    public int checkX;
    public int checkY;
    public MapData _targetMapData;

    // 현재 로드된 맵 데이터를 설정 (MapManager 등에서 호출해주거나 직접 참조)
    public void SetMapData(MapData data)
    {
        _targetMapData = data;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || _targetMapData == null) return;

        // 1. 실제 타일 영역(Actual Bounds) 계산
        // Grid 중심점 좌표를 구하기 위해 minX, minY를 월드 좌표로 변환
        Vector3 bottomLeft = GridUtils.GridToWorld(new Vector3Int(_targetMapData._minX, _targetMapData._minY, 0));

        // actualWidth, actualHeight는 칸 수이므로, 크기는 (칸 수 * 셀 크기)
        // 여기서는 기본 셀 크기 1을 가정합니다. (GridUtils.CELL_SIZE가 있다면 그것을 곱하세요)
        float width = _targetMapData._actualWidth;
        float height = _targetMapData._actualHeight;

        // 기즈모는 중심점을 기준으로 그려지므로 센터 계산
        Vector3 center = bottomLeft + new Vector3(width / 2f, height / 2f, 0);
        Vector3 size = new Vector3(width, height, 0.1f);

        // 2. 면 그리기
        Gizmos.color = areaColor;
        Gizmos.DrawCube(center, size);

        // 3. 테두리 그리기
        Gizmos.color = outlineColor;
        Gizmos.DrawWireCube(center, size);

        // 4. 시작점(MinX, MinY)에 작은 구체 표시
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bottomLeft, 0.2f);

        Vector3 targetWorldPos = GridUtils.GridToWorld(new Vector3Int(checkX, checkY, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetWorldPos, 0.5f);
    }
}