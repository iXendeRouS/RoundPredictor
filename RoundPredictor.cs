using MelonLoader;
using BTD_Mod_Helper;
using RoundPredictor;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using BTD_Mod_Helper.Extensions;
using System.Collections.Generic;
using System;

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

        if (Settings.PredictRoundsKey.JustPressed())
        {
            LogRoundEmissions(Settings.NumberOfRoundsToPredict);
        }
    }

    private static void LogRoundEmissions(int roundsToPredict)
    {
        var seed = InGame.instance.bridge.GetFreeplayRoundSeed();
        FreeplayRoundManager fr = new(InGame.instance.GetGameModel());
        fr.SetSeed(seed);

        int startRound = Math.Max(InGame.instance.currentRoundId, 140);

        for (int i = 0; i < roundsToPredict; i++)
        {
            int internalRoundId = startRound + i;
            int displayRound = internalRoundId + 1;

            var emissions = fr.GetRoundEmissions(internalRoundId);

            if (emissions.Count == 0)
            {
                MelonLogger.Msg($"No emissions for seed: {seed}, round: {displayRound}");
                continue;
            }

            int badCount = 0;
            int fbadCount = 0;
            string previousBloon = null;
            int count = 0;
            float currentGroupStartTime = -1;
            List<string> formattedOutput = new List<string>();

            foreach (var emission in emissions)
            {
                string currentBloon = emission.bloon;
                float currentTime = emission.time;

                if (currentBloon == previousBloon)
                {
                    count++;
                }
                else
                {
                    if (previousBloon != null)
                    {
                        formattedOutput.Add($"{currentGroupStartTime / 60,6:F2}s | {previousBloon,-18} | {count,-5} |");
                    }
                    previousBloon = currentBloon;
                    count = 1;
                    currentGroupStartTime = currentTime;
                }

                if (currentBloon.Equals("Bad")) badCount++;
                else if (currentBloon.Equals("BadFortified")) fbadCount++;
            }

            if (previousBloon != null)
            {
                formattedOutput.Add($"{currentGroupStartTime / 60,6:F2}s | {previousBloon,-18} | {count,-5} |");
            }

            MelonLogger.Msg("--------------------------------------");
            MelonLogger.Msg($"Seed: {seed} | Round: {displayRound}");

            if (Settings.LogBads)
            {
                MelonLogger.Msg($"Bads: {badCount} | FBads: {fbadCount}");
            }

            if (Settings.LogMultipliers)
            {
                MelonLogger.Msg($"Speed: x{GetSpeedMultiplier(displayRound):F2} | Health: x{GetHealthMultiplier(displayRound):F2}");
            }

            MelonLogger.Msg("--------------------------------------");
            MelonLogger.Msg("  Time  | Bloon              | Count |");
            MelonLogger.Msg("--------------------------------------");

            foreach (var line in formattedOutput)
            {
                MelonLogger.Msg(line);
            }

            MelonLogger.Msg("--------------------------------------");
            MelonLogger.Msg("");
        }
    }

    private static double GetSpeedMultiplier(int round)
    {
        if (round <= 80) return 1;
        if (round <= 100) return 1 + (round - 80) * 0.02;
        if (round <= 150) return 1.6 + (round - 101) * 0.02;
        if (round <= 200) return 3.0 + (round - 151) * 0.02;
        if (round <= 250) return 4.5 + (round - 201) * 0.02;
        return 6.0 + (round - 252) * 0.02;
    }

    private static double GetHealthMultiplier(int round)
    {
        if (round <= 80) return 1;
        if (round <= 100) return (round - 30) / 50.0;
        if (round <= 124) return (round - 72) / 20.0;
        if (round <= 150) return ((3 * round) - 320) / 20.0;
        if (round <= 250) return ((7 * round) - 920) / 20.0;
        if (round <= 300) return round - 208.5;
        if (round <= 400) return ((3 * round) - 717) / 2.0;
        if (round <= 500) return ((5 * round) - 1517) / 2.0;
        return (5 * round) - 2008.5;
    }
}