using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

using System.Linq;

namespace GameClient
{
    public class MainMenuPatches
    {
        //This is a special patch called "transpile"
        //It basically alters the opcode so that we can use,
        //and altar, the inside of the rimworld dlls

        //This transpile creates a button at the top of 
        //the menu screen called Rimworld Together
        [HarmonyPatch(typeof(MainMenuDrawer))]
        [HarmonyPatch("DoMainMenuControls")]
        private static class DoMainMenuControlsPatch
        {

            //to be honest, i still don't really know how all this works,
            //but I was able to borrow code and get it to work - Anau Naga
            public static float addedHeight = 45f + 7f;
            public static List<ListableOption> OptionList;
            private static MethodInfo ListingOption = SymbolExtensions.GetMethodInfo(() => AdjustList(null));

            static void AdjustList(List<ListableOption> optList)
            {
                optList.Insert(0, new ListableOption("Rimworld Together", delegate
                {
                    if (!(Network.isConnectedToServer || Network.isTryingToConnect)) DialogShortcuts.ShowConnectDialogs();
                }, null));
                OptionList = optList;
            }

            static bool Prefix(ref Rect rect, bool anyMapFiles)
            {
                rect = new Rect(rect.x, rect.y, rect.width, rect.height + addedHeight);
                return true;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var m_DrawOptionListing =
                    SymbolExtensions.GetMethodInfo(() => OptionListingUtility.DrawOptionListing(Rect.zero, null));

                var instructionsList = instructions.ToList();
                var patched = false;
                for (var i = 0; i < instructionsList.Count; i++)
                {
                    var instruction = instructionsList[i];
                    if (i + 2 < instructionsList.Count)
                    {
                        var checkingIns = instructionsList[i + 2];
                        if (!patched && checkingIns != null && checkingIns.Calls(m_DrawOptionListing))
                        {
                            yield return new CodeInstruction(OpCodes.Ldloc_2);
                            yield return new CodeInstruction(OpCodes.Call, ListingOption);
                            patched = true;
                        }
                    }

                    yield return instruction;
                }
            }
        }

    }
}