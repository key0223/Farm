using SuperTiled2Unity.Editor;
using System;
using System.Collections.Generic;
using static Define;

namespace Data
{
    [Serializable]
    public class ItemLoader : ILoader<int, ItemDataBase>
    {
        public ObjectDataBase[] array;

        public Dictionary<int, ItemDataBase> MakeDict()
        {
            Dictionary<int, ItemDataBase>  dict = new Dictionary<int, ItemDataBase>();
            foreach(ItemDataBase item in array)
            {
                dict.Add(item.Id, item);
            }
            return dict;
        }
    }
    [Serializable]
    public class ItemToolLoader : ILoader<int, ItemDataBase>
    {
        public ToolDataBase[] array;

        public Dictionary<int, ItemDataBase> MakeDict()
        {
            Dictionary<int, ItemDataBase>  dict = new Dictionary<int, ItemDataBase>();
            foreach(ToolDataBase item in array)
            {
                item.UpgradeFrom = Parser.ParseUpgradFrom(item.SalePrice, item.UpgradeFromStr);
                dict.Add(item.Id, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class CropLoader : ILoader<int, CropDataBase>
    {
        public CropDataBase[] array;

        public Dictionary<int, CropDataBase> MakeDict()
        {
            Dictionary<int, CropDataBase> dict = new Dictionary<int, CropDataBase>();
            foreach (CropDataBase item in array)
            {
                //item.Seasons = Parser.ParseEnum<Season>(item.SeasonsStr);
                //item.DaysInPhase = Parser.ParseInt(item.DaysInPhaseStr);
                //item.HarvestMethod = Parser.ParseEnum<ToolType>(item.HarvestMethodStr);
                dict.Add(item.Id, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class  AnimationLoader : ILoader<string, AnimationDataBase>
    {
        public AnimationDataBase[] array;

        public Dictionary<string, AnimationDataBase> MakeDict()
        {
            Dictionary<string, AnimationDataBase> dict = new Dictionary<string, AnimationDataBase>();
            foreach (AnimationDataBase item in array)
            {
                item.SpriteIndex = Parser.ParseMinMaxRange(item.SpriteIndexStr);
                dict.Add(item.AnimationName, item);
            }
            return dict;
        }
    }
    [Serializable]
    public class StringLoader : ILoader<string, StringDataBase>
    {
        public StringDataBase[] array;

        public Dictionary<string, StringDataBase> MakeDict()
        {
            Dictionary<string, StringDataBase> dict = new Dictionary<string, StringDataBase>();
            foreach (StringDataBase item in array)
            {
                dict.Add(item.StringId, item);
            }
            return dict;
        }
    }
    [Serializable]
    public class DialogueLoader : ILoader<string, DialogueData>
    {
        public DialogueData[] array;

        public Dictionary<string, DialogueData> MakeDict()
        {
            Dictionary<string, DialogueData> dict = new Dictionary<string, DialogueData>();
            foreach (DialogueData item in array)
            {
                dict.Add(item.DialogueId, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class ScheduleLoader : ILoader<string, List<ScheduleData>>
    {
        public ScheduleDataBase[] array;

        public Dictionary<string, List<ScheduleData>> MakeDict()
        {
            Dictionary<string, List<ScheduleData>> dict = new Dictionary<string, List<ScheduleData>>();
            foreach(ScheduleDataBase item in array)
            {
                item.scheduleDatas = Parser.ParseRawSchedule(item.ScheduleString);
                dict.Add(item.ScheduleId, item.scheduleDatas);
            }
            return dict;
        }
        
    }

}