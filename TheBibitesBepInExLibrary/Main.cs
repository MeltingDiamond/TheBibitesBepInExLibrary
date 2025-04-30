using UnityEngine;
using HarmonyLib;
using UIScripts.SettingHandles;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace TheBibitesBepInExLibrary
{
    public class Main
    {
        // This should not be registered as a mod on start since this is a library and does nothing on it's own

        private Harmony harmony;

        /// <summary>
        /// What kind of setting element to add.
        /// </summary>
        public enum UIElement
        {
            Toggle,
            Slider,
        }

        /// <summary>
        /// What type of setting it is.
        /// <summary>
        public enum SettingTypes
        {
            SimulationOptions,
            CheatOptions,
            GlobalParameters,
            TPSParameters,
            PhysicsParameters,
            VirginBibiteOptions,
            BibiteConstants,
            MaterialParameters
        }

        /// <summary>
        /// Info for a setting to be added dynamically.
        /// </summary>
        public class SettingDefinition
        {
            public UIElement ElementType;
            public string Label;
            public float Min;
            public float Max;
            public Func<float> Getter;
            public Action<float> Setter;
        }

        // Stores pending settings to add
        internal static List<SettingDefinition> pendingSettings = new List<SettingDefinition>();

        /// <summary>
        /// Call this to define and schedule a UI element to be added to the settings menu.
        /// </summary>
        public void AddSetting(SettingDefinition def, SettingTypes settingType)
        {
            if (harmony == null)
            {
                harmony = new Harmony("TheBibitesBepInExLibrary");

                MethodInfo targetMethod = AccessTools.Method(typeof(MenuDynamicSettingsManager), "Awake");
                MethodInfo postfixMethod = AccessTools.Method(typeof(DynamicSettingsPatch), "Postfix");

                harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfixMethod));

                Debug.Log("DynamicSettingsPatch applied.");
            }

            pendingSettings.Add(def);
            Debug.Log($"Queued setting: {def.Label}");
        }
    }

    public static class DynamicSettingsPatch
    {
        public static void Postfix(MenuDynamicSettingsManager __instance)
        {
            Debug.Log("Patching in queued settings...");

            foreach (var def in Main.pendingSettings)
            {
                switch (def.ElementType)
                {
                    case Main.UIElement.Slider:
                        var slider = __instance.CreateSlider(def.Label, def.Min, def.Max, def.Getter(), def.Setter);
                        break;

                    case Main.UIElement.Toggle:
                        var toggle = __instance.CreateToggle(def.Label, def.Getter() > 0, b => def.Setter(b ? 1 : 0));
                        break;
                }

                Debug.Log($"Added {def.ElementType} setting: {def.Label}");
            }
        }
    }
}