using MelonLoader;
using BTD_Mod_Helper;
using RoundPredictor;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using BTD_Mod_Helper.Extensions;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Utils;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
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

        if (Settings.actionKey.JustPressed())
        {
            var seed = InGame.instance.bridge.GetFreeplayRoundSeed();
            var round = 249;// InGame.instance.currentRoundId;

            FreeplayRoundManager fr = new(InGame.instance.GetGameModel());
            fr.SetSeed(seed);

            var emissions = fr.GetRoundEmissions(round);

            if (emissions.Count == 0)
            {
                MelonLogger.Msg($"There doesn't seem to be any emmissions for currentSeed: {seed}, round: {round}");
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
            MelonLogger.Msg($"currentSeed: {seed}, round: {round + 1} bads: {badcount}, budget multiplier: {fr.budgetMultiplierThisRound}");
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

    public override void OnMatchStart()
    {
        base.OnMatchStart();

        var currentSeed = InGame.instance.bridge.GetFreeplayRoundSeed();
        MelonLogger.Msg($"current currentSeed: {currentSeed}");

        if (Settings.seed == -1 || Settings.seed == currentSeed) return;

        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "SeedUtils", $"Do you want to change the current seed {currentSeed} to {Settings.seed.GetValue()}?",
                            new Action(() => SetSeed(Settings.seed)), "Yes", null, "No",
                            Popup.TransitionAnim.Scale, PopupScreen.BackGround.Grey));
    }

    private static void SetSeed(int seed)
    {
        MelonLogger.Msg($"Setting current seed to {seed}");

        MapSaveDataModel? saveModel = InGame.instance.CreateCurrentMapSave(InGame.instance.currentRoundId, InGame.instance.MapDataSaveId);

        saveModel.freeplayRoundSeed = Settings.seed;

        InGame.Bridge.ExecuteContinueFromCheckpoint(InGame.Bridge.MyPlayerNumber, new KonFuze(), ref saveModel, true, false);
        Game.Player.Data.SetSavedMap(saveModel.savedMapsId, saveModel);
    }
}