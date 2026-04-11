using System.Collections.Generic;

public class RecipeRequirementEvaluator 
{
    public bool CanMake(List<Need> needs, Container container)
    {
        foreach (Need need in needs)
        {
            int remaining = need.Count;
            int have = container.TryGetItemStack(need.ItemId);

            if(have<0) return false;
            if (have < need.Count) return false;
        }

        return true;
    }
}
