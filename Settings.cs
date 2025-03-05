using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.ModOptions;

namespace RoundPredictor
{
    public class Settings : ModSettings
    {
        public static readonly ModSettingHotkey PredictRoundsKey = new(UnityEngine.KeyCode.R)
        {
            description = "Press to log the predictions for the next few rounds of freeplay generation."
        };

        public static readonly ModSettingInt NumberOfRoundsToPredict = new(1)
        {
            description = "The number of rounds to predict at a time.",
            min = 1,
            max = 1000
        };

        public static readonly ModSettingBool LogMultipliers = new(false)
        {
            description = "Log additional information like speed and health multipliers per round."
        };

        public static readonly ModSettingBool LogBads = new(true)
        {
            description = "Log the number of BADs and FBADs in a round."
        };
    }
}
