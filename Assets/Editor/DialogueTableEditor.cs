using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueTable))]
public class DialogueTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueTable table = (DialogueTable)target;

        EditorGUILayout.LabelField("파일 저장 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        /* 파일 이름 입력 */
        table.fileName = EditorGUILayout.TextField("File Name (.json)",table.fileName);
        /* 경로 선택 */
        EditorGUILayout.BeginHorizontal();
        table.savePath = EditorGUILayout.TextField("Save Path",table.savePath);

        if (GUILayout.Button("Select folder", GUILayout.Width(90)))
        {
            string folderPath = EditorUtility.OpenFolderPanel("저장될 폴더 선택", "Assets", "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                // 절대 경로를 유니티 프로젝트 내부 상대 경로로 변환
                if (folderPath.Contains(Application.dataPath))
                {
                    table.savePath = "Assets" + folderPath.Replace(Application.dataPath, "");
                }
                else
                {
                    table.savePath = folderPath; // 외부 경로일 경우
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        /* JSON 내보내기  */
        GUI.color = Color.cyan;
        if (GUILayout.Button("JSON 데이터 내보내기 (Export)", GUILayout.Height(35)))
        {
            ExportToJson(table);
        }
        GUI.color = Color.white;

        EditorGUILayout.Space(10);
        base.OnInspectorGUI();
    }

    void ExportToJson(DialogueTable table)
    {
        // 경로 유효성 검사 및 폴더 생성
        if (!Directory.Exists(table.savePath))
        {
            Directory.CreateDirectory(table.savePath);
        }

        string fullPath = Path.Combine(table.savePath, table.fileName);

        string json = JsonUtility.ToJson(table, true);
        File.WriteAllText(fullPath, json);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("저장 완료", $"{fullPath}에 저장되었습니다.", "확인");
    }
}
