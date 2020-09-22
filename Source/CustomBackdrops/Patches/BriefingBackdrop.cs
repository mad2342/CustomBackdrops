using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace CustomBackdrops.Patches
{
    class BriefingBackdrop
    {
        [HarmonyPatch(typeof(Briefing), "Init")]
        public static class Briefing_Init_Patch
        {
            public static void Postfix(Briefing __instance, RawImage ___BackdropImage, Image ___BackdropImageTutorial, Sprite ___BackdropTutorial, List<long> ___TutorialContracts)
            {
                try
                {
                    GameInstance gameInstance = UnityGameInstance.BattleTechGame;
                    Contract activeContract = gameInstance.Combat.ActiveContract;

                    Logger.Info($"[Briefing_Init_POSTFIX] activeContract.Name: {activeContract.Name}");
                    Logger.Info($"[Briefing_Init_POSTFIX] activeContract.mapMood: {activeContract.mapMood}");
                    Logger.Info($"[Briefing_Init_POSTFIX] activeContract.ContractBiome: {activeContract.ContractBiome}");
                    Logger.Info($"[Briefing_Init_POSTFIX] activeContract.TargetSystem: {activeContract.TargetSystem}");
                    Logger.Info($"[Briefing_Init_POSTFIX] activeContract.ContractTypeValue.ID: {activeContract.ContractTypeValue.ID}");

                    bool isTutorialContract = ___TutorialContracts.Contains((long)activeContract.ContractTypeValue.ID);

                    if (isTutorialContract)
                    {
                        return;
                    }



                    StarSystem starSystem = null;
                    int contractTypeId = activeContract.ContractTypeValue.ID;
                    string biomeString = "";

                    if (activeContract.GameContext.HasObject(GameContextObjectTagEnum.TargetStarSystem))
                    {
                        starSystem = (activeContract.GameContext.GetObject(GameContextObjectTagEnum.TargetStarSystem) as StarSystem);
                    }
                    
                    if (starSystem != null)
                    {
                        // Story_5_ServedCold
                        if (contractTypeId == 22)
                        {
                            biomeString = "martianVacuum";
                        }
                        // Story_8_Locura
                        else if (contractTypeId == 26)
                        {
                            biomeString = "polarFrozen";
                        }
                        else
                        {
                            biomeString = activeContract.ContractBiome.ToString();
                        }
                    }
                    else
                    {
                        biomeString = activeContract.ContractBiome.ToString();
                    }



                    string[] backdropPaths = Directory.GetFiles(Path.Combine(CustomBackdrops.ModDirectory, "Backdrops"));
                    System.Random rnd = new System.Random();
                    backdropPaths = backdropPaths.OrderBy(x => rnd.Next()).ToArray();

                    string backdropPath = backdropPaths.FirstOrDefault((bdp => bdp.Contains(biomeString)));

                    if(string.IsNullOrEmpty(backdropPath))
                    {
                        backdropPath = Path.Combine(CustomBackdrops.ModDirectory, "Backdrops/leopard-generic.jpg");
                    }
                    Logger.Info($"[Briefing_Init_POSTFIX] Chosen backdrop: {backdropPath}");



                    Sprite customBackdropSprite = Utilities.SpriteFromDisk(backdropPath);
                    if (customBackdropSprite != null)
                    {
                        ___BackdropImage.gameObject.SetActive(false);
                        ___BackdropImageTutorial.gameObject.SetActive(true);

                        ___BackdropImageTutorial.sprite = customBackdropSprite;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
