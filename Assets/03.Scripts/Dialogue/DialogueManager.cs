using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static Define;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{

    string _currentRawDialogue;
    string[] _dialogueLines;
    int _lineIndex;

    string _selectedResponseId;
    bool _typing;
    bool _isQuestionActive;
    bool _dialogueKilled;


    List<DialogueResponseOption> _currentResponses;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.ManagerReady("DialogueManager");

    }

    
    public void StartDialogue(string dialogueId)
    {
        DialogueData dialogueData;
        TableDataManager.Instance.DialogueDict.TryGetValue(dialogueId, out dialogueData);
        if (dialogueData == null) return;

        _currentRawDialogue = dialogueData.Dialogue;
        _dialogueLines = _currentRawDialogue.Split('#');
        _lineIndex = 0;
        _dialogueKilled = false;
        ProcessNextLine();
    }

    void ProcessNextLine()
    {
        if(_dialogueKilled || _lineIndex >= _dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        string line = _dialogueLines[_lineIndex++].Trim();
        DialogueTagType tag = ParseDialogueTag(line);

        if(tag == DialogueTagType.KILL)
        {
            _dialogueKilled = true;
            EndDialogue();
            return;
        }

        if (ProcessTag(tag, line)) return;

        string parsed = ReplaceTokens(line);

        //StartCoroutine(CoTypeText());
    }
  

    void EndDialogue()
    {
        
    }
    #region Tag Handler

    void ShowQuestion(string line)
    {
        Match match = Regex.Match(line, @"\$q (.*?)#(.*)");
        if(match.Success)
        {
            string dialogue = ReplaceTokens(match.Groups[2].Value);
            ParseResponses(_currentRawDialogue);
            _isQuestionActive = true;
        }
            
    }

    void ParseResponses(string raw)
    {
        _currentResponses = new List<DialogueResponseOption>();

        MatchCollection matches = Regex.Matches(raw, @"#r (\d+) ([+-]\d+) (\w+)#(.*?)($|#)");

        foreach(Match match in matches)
        {
            _currentResponses.Add(new DialogueResponseOption
            {
                ResponseId = match.Groups[1].Value,
                ReactionId = match.Groups[3].Value,
                PlayerResponsText = match.Groups[4].Value,
            });
        }


        
    }
    #endregion
    #region Utils
    IEnumerator CoTypeText(TMP_Text txt, string text)
    {
        txt.text = "";
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
                //ShowQuestion(line);
                return true;
            case DialogueTagType.CHANCE:
                //HandleChance(line);
                return true;
            case DialogueTagType.WORLD:
                //HandleWorldState(line);
                return true;
            case DialogueTagType.PRE_PREREQUISITES:
                //HandlePrerequisite(line);
                return true;
            case DialogueTagType.QUICK:
                //HandleQuickQuestion(line);
                return true;
            case DialogueTagType.ONCE:
                //HandleOnce(line);
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
        if (!line.StartsWith("$q")) return DialogueTagType.QUESTION;
        if (!line.StartsWith("$r")) return DialogueTagType.RESPONSE;
        if (!line.StartsWith("$b")) return DialogueTagType.BREAK;
        if (!line.StartsWith("$e")) return DialogueTagType.END;
        if (!line.StartsWith("$k")) return DialogueTagType.KILL;
        if (!line.StartsWith("$c")) return DialogueTagType.CHANCE;
        if (!line.StartsWith("$d")) return DialogueTagType.WORLD;
        if (!line.StartsWith("$p")) return DialogueTagType.PRE_PREREQUISITES;
        if (!line.StartsWith("$y")) return DialogueTagType.QUICK;
        if (!line.StartsWith("$1")) return DialogueTagType.ONCE;

        return DialogueTagType.NONE;
    }
    #endregion
}
