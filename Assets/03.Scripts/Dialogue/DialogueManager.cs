using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static Define;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    InputState _input;

    string _currentNpc;

    string _currentRawDialogue;
    string[] _dialogueLines;
    int _lineIndex;

    string _selectedResponseId;
    bool _typing;
    bool _isQuestionActive;
    bool _dialogueKilled;

    DialogueState _dialogueState;
    List<DialogueResponseOption> _currentResponses = new List<DialogueResponseOption>();

    protected override void Awake()
    {
        base.Awake();
        _dialogueState = new DialogueState();
        GameManager.Instance.ManagerReady("DialogueManager");
    }

    void Start()
    {
        _input = InputManager.Instance.InputState;
    }

    void Update()
    {
        if (!UIManager.Instance.DialogueUI.gameObject.activeSelf) return;

        if (_input.IsNewKeyPress(Keys.Return) && !_typing && !_isQuestionActive)
            ProcessNextLine();
    }


    public void StartDialogue(string npc, string dialogueId)
    {
        DialogueData dialogueData;
        TableDataManager.Instance.DialogueDict.TryGetValue(dialogueId, out dialogueData);
        if (dialogueData == null) return;

        _currentNpc = npc;
        _currentRawDialogue = dialogueData.Dialogue;
        _dialogueLines = _currentRawDialogue.Split('#')
         .Where(l => !string.IsNullOrWhiteSpace(l.Trim())).Select(l => l.Trim()).ToArray();
        _lineIndex = 0;
        _dialogueKilled = false;
        UIManager.Instance.ShowDialogue();
        ProcessNextLine();
    }

    void ProcessNextLine()
    {
        if (_dialogueKilled || _lineIndex >= _dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        string line = _dialogueLines[_lineIndex].Trim();
        DialogueTagType tag = ParseDialogueTag(line);

        _lineIndex++;

        if (tag == DialogueTagType.KILL)
        {
            _dialogueKilled = true;
            EndDialogue();
            return;
        }

        if (ProcessTag(tag, line)) return;

        string parsed = ReplaceTokens(line);
        StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, parsed));
    }


    void EndDialogue()
    {
        _dialogueState.ClearSession();
        UIManager.Instance.HideDialogue();
        _currentResponses.Clear();
    }
    #region Question Dialogue

    void ShowQuestion(string line)
    {
        Match qMatch = Regex.Match(line, @"\$q\s*([0-9,/]+)\s+(\w+)");
        if (qMatch.Success)
        {
            string idsRaw = qMatch.Groups[1].Value.Replace("/", ","); // "17/18" → "17,18"
            string fallbackKey = qMatch.Groups[2].Value;

            string[] ids = idsRaw.Split(',').Select(s => s.Trim()).ToArray();
            bool alreadyAnswered = ids.Any(id => _dialogueState.HasChosenResponse(_currentNpc, id));

            if (alreadyAnswered)
            {
                LoadReactionDialogue(fallbackKey);
                return;
            }
        }

        ClearResponseButtons();
        _currentResponses.Clear();

        string questionText = GetNextPlainText();

        StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, questionText));
        ParseResponses(_currentRawDialogue);
        SetButtons();
        _isQuestionActive = true;

    }
    string GetNextPlainText()
    {
        for (int i = _lineIndex; i < _dialogueLines.Length; i++)
        {
            string nextLine = _dialogueLines[i].Trim();
            if (!nextLine.StartsWith("$"))
            {
                return ReplaceTokens(nextLine);
            }
        }
        return "No Question...";
    }

    void ParseResponses(string raw)
    {
        if (_currentResponses == null)
            _currentResponses = new List<DialogueResponseOption>();

        _currentResponses.Clear();


        var matches = Regex.Matches(raw,
        @"#\$r\s+(\d+)\s+([+-]?\d*)\s+(\w+)\s*#([^#]+)",
        RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            _currentResponses.Add(new DialogueResponseOption
            {
                ResponseId = match.Groups[1].Value,
                ReactionId = match.Groups[3].Value,
                PlayerResponsText = ReplaceTokens(match.Groups[4].Value.Trim())
            });
        }

    }

    void SetButtons()
    {
        UIManager.Instance.DialogueUI.SetButtonInactive();
        int count = Mathf.Min(_currentResponses.Count, UIManager.Instance.DialogueUI.Buttons.Count);

        for (int i = 0; i < count; i++)
        {
            DialogueResponseOption resp = _currentResponses[i];
            ResponseButton button = UIManager.Instance.DialogueUI.Buttons[i];
            button.SetResponseText(resp.PlayerResponsText, OnResponseSelected);
            button.gameObject.SetActive(true);
        }

        UIManager.Instance.ActiveMenu.PopulateClickableComponentList();

    }

    void OnResponseSelected(int index)
    {
        SelectResponse(index);
    }

    void SelectResponse(int index)
    {
        DialogueResponseOption response = _currentResponses[index];
        _dialogueState.ChooseResponse(_currentNpc, response.ResponseId);

        ClearResponseButtons();
        _isQuestionActive = false;
        StopAllCoroutines();
        UIManager.Instance.DialogueUI.DialogueText.text = "";
        LoadReactionDialogue(response.ReactionId);
    }

    void LoadReactionDialogue(string reactionId)
    {
        DialogueData reactionData;
        TableDataManager.Instance.DialogueDict.TryGetValue(reactionId, out reactionData);
        if (reactionData == null)
        {
            ProcessNextLine();
            return;
        }

        _currentRawDialogue = reactionData.Dialogue;
        _dialogueLines = _currentRawDialogue.Split('#');
        _lineIndex = 0;
        ProcessNextLine();
    }
    void ClearResponseButtons()
    {
        UIManager.Instance.DialogueUI.SetButtonInactive();
    }

    #endregion


    void HandlePrerequisite(string line)
    {
        int hashIdx = line.IndexOf('#');
        if (hashIdx == -1)
        {
            // $p 17 다음 일반 텍스트 처리
            ProcessPWithNextText(line);
            return;
        }

        string idPart = line.Substring(3, hashIdx - 3).Trim(); // $p 다음부터 # 전까지
        string[] ids = idPart.Split('/').Select(s => s.Trim()).ToArray();
        string textPart = line.Substring(hashIdx + 1).Trim();
        string[] branches = textPart.Split('|', 2);

        bool matched = ids.Any(id => _dialogueState.HasChosenResponse(_currentNpc, id));
        string chosen = matched && branches.Length > 0 ? branches[0] : (branches.Length > 1 ? branches[1] : textPart);
        StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, ReplaceTokens(chosen)));

    }
    void ProcessPWithNextText(string idLine)
    {
        string idPart = idLine.Substring(3).Trim();
        string[] ids = idPart.Split('/').Select(s => s.Trim()).ToArray();

        for (int i = _lineIndex; i < _dialogueLines.Length; i++)
        {
            string nextLine = _dialogueLines[i].Trim();
            if (!nextLine.StartsWith("$") && !string.IsNullOrEmpty(nextLine))
            {
                string[] branches = nextLine.Split('|', 2);
                bool matched = ids.Any(id => _dialogueState.HasChosenResponse(_currentNpc, id));
                string chosen = matched && branches.Length > 0 ? branches[0] : (branches.Length > 1 ? branches[1] : nextLine);
                StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, ReplaceTokens(chosen)));
                _lineIndex = i + 1;
                return;
            }
        }
    }
    void HandleQuickQuestion(string line)
    {
        Match yMatch = Regex.Match(line, @"\$y '(.*?)'");
        if (!yMatch.Success) return;

        string content = yMatch.Groups[1].Value;
        string[] parts = content.Split('_');

        if (parts.Length < 3) return;

        // 질문
        StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, ReplaceTokens(parts[0])));

        UIManager.Instance.DialogueUI.SetButtonInactive();
        int buttonIdx = 0;

        for (int i = 1; i < parts.Length; i += 2)
        {
            if (i + 1 >= parts.Length) break;

            string choice = ReplaceTokens(parts[i]);
            string reaction = ReplaceTokens(parts[i + 1]);

            ResponseButton button = UIManager.Instance.DialogueUI.Buttons[buttonIdx];
            button.ButtonIndex = buttonIdx;
            button.SetResponseText(choice, () =>
            {
                UIManager.Instance.DialogueUI.SetButtonInactive();
                _isQuestionActive = false;
                StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, reaction));
            });
            button.gameObject.SetActive(true);
            buttonIdx++;
        }

        UIManager.Instance.ActiveMenu.PopulateClickableComponentList();
        _isQuestionActive = true;
    }
    void HandleOnce(string line)
    {
        Match match = Regex.Match(_currentRawDialogue,
            @"(\$1\s+\w+)\s*#([^\$]+?)\s*#\$e\s*#([^\$]+)",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
        {
            ProcessNextLine();
            return;
        }

        string dialogueId = match.Groups[1].Value.Split(' ')[1].Trim();
        string firstText = match.Groups[2].Value.Trim();
        string repeatText = match.Groups[3].Value.Trim();

        bool seen = _dialogueState.HasSeenDialogue(_currentNpc, dialogueId);
        string chosen = seen ? repeatText : firstText;

        if (!seen)
            _dialogueState.SetDialogueSeen(_currentNpc, dialogueId);

        StartCoroutine(CoTypeText(UIManager.Instance.DialogueUI.DialogueText, ReplaceTokens(chosen)));

        _lineIndex++; // $e 스킵
    }
    #region Utils
    IEnumerator CoTypeText(TextMeshProUGUI txt, string text)
    {
        txt.text = "";
        _typing = true;
        foreach (char c in text)
        {
            txt.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        _typing = false;
    }
    bool ProcessTag(DialogueTagType type, string line)
    {
        switch (type)
        {
            case DialogueTagType.BREAK:
                // Space 대기 후 다음 라인
                return true;
            case DialogueTagType.END:
                EndDialogue();
                return true;
            case DialogueTagType.QUESTION:
                ShowQuestion(line);
                return true;
            case DialogueTagType.CHANCE:
                //HandleChance(line);
                return true;
            case DialogueTagType.WORLD:
                //HandleWorldState(line);
                return true;
            case DialogueTagType.PRE_PREREQUISITES:
                HandlePrerequisite(line);
                return true;
            case DialogueTagType.QUICK:
                HandleQuickQuestion(line);
                return true;
            case DialogueTagType.ONCE:
                HandleOnce(line);
                return true;
        }
        return false;
    }

    string ReplaceTokens(string line)
    {
        line = Regex.Replace(line, @"\$b|\$e|\$k", "");
        line = line.Replace("@", "플레이어");
        return line;
    }
    DialogueTagType ParseDialogueTag(string line)
    {
        if (!line.StartsWith("$")) return DialogueTagType.NONE;

        if (line.StartsWith("$q")) return DialogueTagType.QUESTION;
        if (line.StartsWith("$r")) return DialogueTagType.RESPONSE;
        if (line.StartsWith("$b")) return DialogueTagType.BREAK;
        if (line.StartsWith("$e")) return DialogueTagType.END;
        if (line.StartsWith("$k")) return DialogueTagType.KILL;
        if (line.StartsWith("$c")) return DialogueTagType.CHANCE;
        if (line.StartsWith("$d")) return DialogueTagType.WORLD;
        if (line.StartsWith("$p")) return DialogueTagType.PRE_PREREQUISITES;
        if (line.StartsWith("$y")) return DialogueTagType.QUICK;
        if (line.StartsWith("$1")) return DialogueTagType.ONCE;

        return DialogueTagType.NONE;
    }
    #endregion
}
