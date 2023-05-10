using _Core.Interfaces;
using UnityEngine;
using Steamworks;

namespace _Core.Common
{
    public class AchievementsManager : Singleton<AchievementsManager>
    {
        public static void SetAsComplete(Achievements achievement)
        {
#if UNITY_EDITOR
            Debug.Log($"You got the {achievement.ToString()} achievement!");
#else
            // Get the user's Steam ID
            CSteamID steamID = SteamUser.GetSteamID();

            // Get the achievement's ID
            string achievementID = achievement.ToString();

            // Unlock the achievement for the user
            SteamUserStats.SetAchievement(achievementID);
            SteamUserStats.StoreStats();
#endif
        }
    }
}


