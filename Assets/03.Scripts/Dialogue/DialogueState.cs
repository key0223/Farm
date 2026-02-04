using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState 
{
    Dictionary<string, HashSet<string>> _npcResponses = new Dictionary<string, HashSet<string>>(); // 저장 될 데이터
    Dictionary<string, HashSet<string>> _eventDialogue = new Dictionary<string, HashSet<string>>(); // 한 번만 보여줄 대사

    HashSet<string> _sessionResponses = new HashSet<string>();
    HashSet<string> _sessionDialogues = new HashSet<string>();

    public bool HasChosenResponse(string npcId, string responseId)
    {
        return _npcResponses.ContainsKey(npcId) && _npcResponses[npcId].Contains(responseId);
    }
   
    public void ChooseResponse(string npcId, string responseId)
    {
        if (!_npcResponses.ContainsKey(npcId))
            _npcResponses[npcId] = new HashSet<string>();

        _npcResponses[npcId].Add(responseId);
        _sessionResponses.Add(responseId);

    }

    public bool HasSeenDialogue(string npcId, string dialogueId)
    {
        return _eventDialogue.ContainsKey(npcId) && _eventDialogue[npcId].Contains(dialogueId);
    }

    public void SetDialogueSeen(string npcId, string dialogueId)
    {
        if (!_eventDialogue.ContainsKey(npcId))
            _eventDialogue[npcId] = new HashSet<string>();

        _eventDialogue[npcId].Add(dialogueId);
        _sessionDialogues.Add(dialogueId);
    }

    public void ClearSession()
    {
        _sessionResponses.Clear();
        _sessionDialogues.Clear();
    }
}
