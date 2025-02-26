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
    }
}
