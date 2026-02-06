using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public static class Parser
{
    public static HashSet<string> ParseString(string rawString)
    {
        HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase); /* 중복, 대소문자 무시 */
        if (string.IsNullOrWhiteSpace(rawString)) return set; /* 입력 값이 null 이면 return */
        var removeSpace = new string(rawString.Where(ch => !char.IsWhiteSpace(ch)).ToArray()); /* 공백 제거 */

        int start = 0;
        for (int i = 0; i <= removeSpace.Length; i++)
        {
            bool isEnd = (i == removeSpace.Length);
            bool isComma = (!isEnd && removeSpace[i] == ',');

            if (isEnd || isComma)
            {
                int len = i - start;
                if (len > 0)
                {
                    string tag = removeSpace.AsSpan(start, len).ToString().ToLowerInvariant();
                    if (tag.Length > 0)
                        set.Add(tag);

                    start = i + 1;

                }
            }
        }
        return set;
    }

    public static HashSet<TEnum> ParseEnum<TEnum>(string rawString) where TEnum : struct, Enum
    {
        HashSet<TEnum> set = new HashSet<TEnum>();

        if (string.IsNullOrWhiteSpace(rawString)) return set; /* 입력 값이 null 이면 return */
        var removeSpace = new string(rawString.Where(ch => !char.IsWhiteSpace(ch)).ToArray()); /* 공백 제거 */

        int start = 0;
        for (int i = 0; i <= removeSpace.Length; i++)
        {
            bool isEnd = (i == removeSpace.Length);
            bool isComma = (!isEnd && removeSpace[i] == ',');

            if (isEnd || isComma)
            {
                int len = i - start;
                if (len > 0)
                {
                    string token = removeSpace.AsSpan(start, len).ToString().ToLowerInvariant();

                    if (token.Length > 0 && Enum.TryParse<TEnum>(token, ignoreCase: true, out var type))
                        set.Add(type);
                    start = i + 1;

                }
            }
        }
        return set;
    }
    public static List<int> ParseInt(string rawString)
    {
        List<int> set = new List<int>();

        if (string.IsNullOrWhiteSpace(rawString)) return set;
        var removeSpace = new string(rawString.Where(ch => !char.IsWhiteSpace(ch)).ToArray());

        int start = 0;
        for (int i = 0; i <= removeSpace.Length; i++)
        {
            bool isEnd = (i == removeSpace.Length);
            bool isComma = (!isEnd && removeSpace[i] == ',');

            if (isEnd || isComma)
            {
                int len = i - start;
                if (len > 0)
                {
                    string token = removeSpace.AsSpan(start, len).ToString();

                    if (token.Length > 0 && int.TryParse(token, out var value))
                        set.Add(value);

                    start = i + 1;
                }
            }
        }

        return set;
    }
    public static List<int> ParseMinMaxRange(string rawString)
    {
        List<int> set = new List<int>();

        if (string.IsNullOrWhiteSpace(rawString)) return set;

        string[] ranges = rawString.Split(',').Select(s => s.Trim()).ToArray();

        foreach (string range in ranges)
        {
            if (range.Contains('-'))
            {
                string[] tokens = range.Split('-');
                /* "23-32" 0 = 23(min), 1 = 32(max) */
                if (tokens.Length == 2 && int.TryParse(tokens[0], out int min) && int.TryParse(tokens[1], out int max))
                {
                    for (int value = min; value <= max; value++)
                        set.Add(value);
                }
            }
            else if (int.TryParse(range, out int single))
            {
                set.Add(single);
            }
        }
        /* result = [23,24,25,26,27,28,29,30,31,32] */
        return set;
    }

    public static UpgradeFrom ParseUpgradFrom(int price, string rawString)
    {
        if (string.IsNullOrWhiteSpace(rawString)) return null;
        string[] tokens = rawString.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length < 3)
            Debug.LogWarning($"UpgradeFrom string needs 3 fields, but got {tokens.Length}. raw={rawString}");

        return new UpgradeFrom
        {
            RequireToolId = int.Parse(tokens[0]),
            Price = price,
            TradeItemId = int.Parse(tokens[1]),
            TradeItemAmount = int.Parse(tokens[2]),
        };
    }

    public static Color ParseColor(string color)
    {
        Color newColor;

        if (ColorUtility.TryParseHtmlString(color, out newColor))
            return newColor;

        return new Color(0, 0, 0);
    }
    public static bool TryParseTileKey(string tileKey, out Vector3Int gridPos)
    {
        gridPos = Vector3Int.zero;

        if (tileKey.StartsWith("x") && tileKey.Contains("y"))
        {
            string trimmed = tileKey.Substring(1); // "5y3"
            string[] xy = trimmed.Split("y");

            if (xy.Length == 2 && int.TryParse(xy[0], out int x)
                              && int.TryParse(xy[1], out int y))
            {
                gridPos = new Vector3Int(x, y, 0);
                return true;
            }
        }

        return false;
    }

    public static List<ScheduleData> ParseRawSchedule(string rawSchedule)
    {
        List<ScheduleData> scheduleDatas = new List<ScheduleData>();
        string[] parts = rawSchedule.Split('/');
        foreach (string part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;
            string[] fields = part.Split(' ');
            scheduleDatas.Add(new ScheduleData
            {
                Time = int.Parse(fields[0]),
                Location = fields[1],
                TargetX = int.Parse(fields[2]),
                TargetY = int.Parse(fields[3]),
                Facing = int.Parse(fields[4]),
                Animation = fields.Length > 5 ? fields[5] : "idle",
                DialogueId = fields.Length > 6 ? fields[6] : ""
            });
        }

        return scheduleDatas.OrderBy(t => t.Time).ToList();
    }
}
