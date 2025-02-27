using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.ModOptions;

namespace RoundPredictor
{
    public class Settings : ModSettings
    {
        public static readonly ModSettingHotkey actionKey = new(UnityEngine.KeyCode.R)
        {
            description = "Press to log the current round's bloon emissions."
        };

        public static readonly ModSettingInt seed = new ModSettingInt(-1)
        {
            description = "Sets the seed to the given value. -1 to disable",
            min = -1,
            max = 999999999
        };

        public static readonly ModSettingBool autoSetSeedInSandbox = new(true)
        {
            description = "Automatically set the seed in sandbox"
        };
    }
}
