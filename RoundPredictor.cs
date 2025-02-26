using MelonLoader;
using BTD_Mod_Helper;
using RoundPredictor;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using BTD_Mod_Helper.Extensions;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(RoundPredictor.RoundPredictor), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace RoundPredictor;

public class RoundPredictor : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<RoundPredictor>("RoundPredictor loaded!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (InGame.instance == null) return;

        if (Settings.actionKey.JustPressed())
        {
            var seed = InGame.instance.bridge.GetFreeplayRoundSeed();
            var round = InGame.instance.currentRoundId;

            FreeplayRoundManager fr = new FreeplayRoundManager(InGame.instance.GetGameModel());
            fr.SetSeed(seed);

            var emissions = fr.GetRoundEmissions(round);

            if (emissions.Count == 0)
            {
                MelonLogger.Msg($"There doesn't seem to be any emmissions for seed: {seed}, round: {round}");
                return;
            }

            int badcount = 0;
            string previousBloon = null;
            int count = 0;

            List<string> formattedOutput = new List<string>();

            foreach (var emission in emissions)
            {
                string currentBloon = emission.bloon;

                if (currentBloon == previousBloon)
                {
                    count++;
                }
                else
                {
                    if (previousBloon != null)
                    {
                        formattedOutput.Add($"{previousBloon,-18} | {count,-5} |");
                    }

                    previousBloon = currentBloon;
                    count = 1;
                }

                if (currentBloon.Contains("Bad"))
                {
                    badcount++;
                }
            }

            if (previousBloon != null)
            {
                formattedOutput.Add($"{previousBloon,-18} | {count,-5} |");
            }

            MelonLogger.Msg("--------------------------------------");
            MelonLogger.Msg($"seed: {seed}, round: {round + 1} bads: {badcount}");
            MelonLogger.Msg("--------------------------------------");
            MelonLogger.Msg("Bloon              | Count |");
            MelonLogger.Msg("--------------------------------------");

            foreach (var line in formattedOutput)
            {
                MelonLogger.Msg(line);
            }

            MelonLogger.Msg("--------------------------------------");
        }
    }
}