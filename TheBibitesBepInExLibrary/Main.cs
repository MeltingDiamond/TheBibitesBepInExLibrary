using UnityEngine;
using HarmonyLib;
using UIScripts.SettingHandles;
using System.Reflection;

namespace TheBibitesBepInExLibrary
{
    public class Main
    {
        // This should not be registered as a mod on start since this is a library and does nothing on it's own

        private Harmony harmony;

        public enum UIElement // Maybe usefull
        {
            Toggle,
            Slider,
            Simulation
        }

        /// <summary>
        /// Adds a UI slider to the choosen location. The locations you can are:
        /// SimulationOptions,
        /// CheatOptions,
        /// GlobalParameters,
        /// TPSParameters,
        /// PhysicsParameters,
        /// VirginBibiteOptions,
        /// BibiteConstants,
        /// MaterialParameters
        /// </summary>
        public void AddSlider(UIElement Element)
        {
            if (harmony != null) return;

            harmony = new Harmony("com.thebibites.dynamicui");

            MethodInfo targetMethod = AccessTools.Method(typeof(MenuDynamicSettingsManager), "Awake");
            MethodInfo postfixMethod = AccessTools.Method(typeof(DynamicSettingsPatch), "Postfix");

            harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfixMethod));

            Debug.Log("Dynamic UI slider patch applied.");
        }
    }

    [HarmonyPatch(typeof(MenuDynamicSettingsManager), "Awake")] // Change this to the actual method where settings are set up
    public static class DynamicSettingsPatch
    {
        // patches in UI elements in the Dynamic Settings
        public static void Postfix(MenuDynamicSettingsManager __instance)
        {
            Debug.Log("Patching in UI elements in various settings");
        }
    }
}