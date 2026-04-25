using SuperTiled2Unity.Editor.ClipperLib;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
[CustomPropertyDrawer(typeof(DialogueData))]
public class DialogueDataDrawer : PropertyDrawer
{
    const float Padding = 2f;
    const float TextAreaHeight = 60f;
    const float ButtonHeight = 22f;
    const float VerticalSpace = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty idProp = property.FindPropertyRelative("DialogueId");
        SerializedProperty ownerProp = property.FindPropertyRelative("DialogueOwner");
        SerializedProperty nameProp = property.FindPropertyRelative("DialogueName");
        SerializedProperty textProp = property.FindPropertyRelative("Dialogue");

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();

        float currentY = position.y;
        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect fieldRect = new Rect(position.x, position.y, position.width, lineHeight);

        GUI.enabled = false; // ID 필드 수정 불가하게 만들기
        EditorGUI.PropertyField(fieldRect, idProp);
        GUI.enabled = true;

        /* 필드 출력 */
        fieldRect.y += lineHeight + Padding;
        EditorGUI.PropertyField(fieldRect, ownerProp);
        fieldRect.y += lineHeight + Padding;
        EditorGUI.PropertyField(fieldRect, nameProp);

        if (EditorGUI.EndChangeCheck())
        {
            // DialogueOwner_DialogueName 형식으로 자동 조합
            idProp.stringValue = $"{ownerProp.stringValue}_{nameProp.stringValue}";
        }

        /* 대사 입력 */
        fieldRect.y += lineHeight + 5f;
        EditorGUI.LabelField(fieldRect, "Dialogue (아래 버튼으로 태그 추가)", EditorStyles.miniBoldLabel);

        fieldRect.y += lineHeight;
        fieldRect.height = TextAreaHeight;
        textProp.stringValue = EditorGUI.TextArea(fieldRect, textProp.stringValue);


        /* 태그 삽입 버튼 배치 */
        Rect btnAreaRect = new Rect(position.x, fieldRect.yMax + Padding, position.width, ButtonHeight);
        DrawResponsiveButtons(btnAreaRect, textProp);

        EditorGUI.EndProperty();
    }

    void DrawResponsiveButtons(Rect rect, SerializedProperty prop)
    {
        string[] tags = 
            { 
            "$q (질문)", 
            "$r (응답)", 
            "$b (끊기)", 
            "$e (종료)", 
            "$k (Kill)", 
            "$c (Change)", 
            "$d (World)",
            "$p (조건)", 
            "$y (빠른)", 
            "$1 (최초)", 
            "@ (이름)",
        };
        string[] contents = 
            {
            "#$q 0/0 fallback#",
            "#$r Id 호감도 연결대사ID#대사",
            "#$b#",
            "#$e#",
            "#$k#",
            "$c (Change)",
            "$d (World)",
            "$p 응답ID#참|거짓", 
            "$y '질문'_선택1_반응대사1_선택2_반응대사2",
            "$1 OnceId#최초대사#반복대사",
            "@",
        };
        //Color[] colors = { new Color(0.7f, 0.8f, 1f), new Color(0.7f, 1f, 0.7f), new Color(1f, 0.9f, 0.7f), new Color(1f, 0.7f, 0.7f), Color.white };

        float spacing = 3f;
        int totalButtons = tags.Length;

        // 인스펙터 너비에 따라 줄당 버튼 개수 결정 (300px 기준)
        int buttonsPerRow = (rect.width < 300f) ? 2 : 5;
        float buttonWidth = (rect.width - (spacing * (buttonsPerRow - 1))) / buttonsPerRow;

        for (int i = 0; i < totalButtons; i++)
        {
            int row = i / buttonsPerRow;
            int col = i % buttonsPerRow;

            Rect btnRect = new Rect(
                rect.x + (col * (buttonWidth + spacing)),
                rect.y + (row * (ButtonHeight + spacing)),
                buttonWidth,
                ButtonHeight
            );

            /* 버튼 */
            //GUI.color = colors[i];
            if (GUI.Button(btnRect, new GUIContent(tags[i])))
            {
                prop.stringValue += contents[i];
            }
        }
        GUI.color = Color.white;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float inspectorWidth = EditorGUIUtility.currentViewWidth;
        int buttonsPerRow = (inspectorWidth < 330f) ? 2 : 4;
        int rows = Mathf.CeilToInt(8f / buttonsPerRow);

        float buttonAreaHeight = rows * (ButtonHeight + 3f);
        return (EditorGUIUtility.singleLineHeight * 4) + TextAreaHeight + buttonAreaHeight + 25f;
    }
}
