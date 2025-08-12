using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using PEAKUnbound.Patches;
using UnityEngine.InputSystem;

namespace PEAKUnbound;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin instance = null!;
    internal static ManualLogSource Log { get; private set; } = null!;
    private readonly Harmony _harmony = new Harmony(Id);

    private static List<ConfigEntry<string>> simpleConfigList = [];
    private static Dictionary<ConfigEntry<string>, int> complexConfigList = [];
    
    private void Awake()
    {
        // Plugin startup logic
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loaded!");
        _harmony.PatchAll(typeof(ControllerSensitivitySettingGetMinMaxValuePatch));
        CreateConfigBindings();

        simpleConfigList.Do(x => Rebind(x));
        complexConfigList.Do(x => RebindComplex(x));

        Config.SettingChanged += OnSettingChanged;
        Config.ConfigReloaded += OnConfigReloaded;
        
    }
    public void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
    {
        if (settingChangedArg.ChangedSetting == null)
            return;

        ConfigEntry<string> settingChanged = (ConfigEntry<string>)settingChangedArg.ChangedSetting;
        
        if (settingChanged == null)
            return;

        Log.LogDebug($"CONFIG SETTING CHANGE EVENT - {settingChangedArg.ChangedSetting.Definition.Key}");
        ConfigEntryRebind(settingChanged);
    }

    public void ConfigEntryRebind(ConfigEntry<string> entryGiven)
    {
        if (!ValidValues.Contains(entryGiven.Value))
            return;
        
        if(simpleConfigList.Contains(entryGiven))
            Rebind(entryGiven);

        if (complexConfigList.ContainsKey(entryGiven))
        {
            var pair = complexConfigList.FirstOrDefault(x => x.Key == entryGiven);
            RebindComplex(pair);
        }    
    }

    public void OnConfigReloaded(object sender, EventArgs e)
    {
        Log.LogDebug("Config has been reloaded!");
    }

    public static void RebindComplex(KeyValuePair<ConfigEntry<string>, int> entryGiven)
    {
        if (entryGiven.Key == null)
        {
            Log.LogError("Null config setting provided!!!");
            return;
        }
        else
            Log.LogDebug($"Processing binding for {entryGiven.Key.Definition.Key}");

        if (!ValidValues.Contains(entryGiven.Key.Value))
        {
            Log.LogWarning($"Unable to bind {entryGiven.Key.Definition.Key} to {entryGiven.Value} (INVALID VALUE)");
            return;
        }

        string actionName = entryGiven.Key.Definition.Key[..(entryGiven.Key.Definition.Key.IndexOf(';'))];
        int index = entryGiven.Value;

        InputAction inputAction = InputSystem.actions.FindAction(actionName);
        SetAction(inputAction, entryGiven.Key.Definition.Key, entryGiven.Key.Value, index);
    }

    //rebind for inputactions that have path at index 0
    public static void Rebind(ConfigEntry<string> entryGiven)
    {
        if (entryGiven == null)
        {
            Log.LogError("Null config setting provided!!!");
            return;
        }
        else
            Log.LogDebug($"Processing binding for {entryGiven.Definition.Key}");

        if (!ValidValues.Contains(entryGiven.Value))
        {
            Log.LogWarning($"Unable to bind {entryGiven.Definition.Key} to {entryGiven.Value} (INVALID VALUE)");
            return;
        }

        InputAction inputAction = InputSystem.actions.FindAction(entryGiven.Definition.Key);
        SetAction(inputAction, entryGiven.Definition.Key, entryGiven.Value);
    }

    public static void SetAction(InputAction inputAction, string Name, string Value, int index = 0)
    {
        if (inputAction == null)
        {
            Log.LogError($"Unable to find inputAction for {Name}");
            return;
        }

        inputAction.ChangeBinding(index).WithPath(Value);
        Log.LogMessage($"Updated binding for {Name} with {Value}");
    }

    public void CreateConfigBindings()
    {
        complexConfigList = [];
        simpleConfigList = [];
        InputSystem.actions.Do(action =>
        {
            if (action.bindings.Count == 0)
                return;

            if (invalidActions.Contains(action.name))
                return;

            Log.LogDebug($"Processing InputAction - {action.name}");

            if (action.bindings.Count > 1)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (!ValidValues.Contains(action.bindings[i].effectivePath))
                        continue;

                    string sectionName = "General";
                    string identifier = "";

                    if (action.bindings[i].effectivePath.Contains("/"))
                    {
                        int firstSep = action.bindings[i].effectivePath.IndexOf('/');
                        sectionName = action.bindings[i].effectivePath[..firstSep].Replace("<", "").Replace(">", "");
                        identifier = action.bindings[i].effectivePath[firstSep..].Replace("/","");
                    }
                    else
                    {
                        if (action.bindings[i].effectivePath == "Dpad")
                            sectionName = "Gamepad";

                        identifier = action.bindings[i].effectivePath;
                    }
                        

                    ConfigEntry<string> actionConfig = Config.Bind(sectionName, action.name + $"; {identifier}", action.bindings[i].effectivePath, $"Change binding for {action.name} ({i}).\nThis is an action with multiple bindings.");
                    complexConfigList.Add(actionConfig, i);
                }
            }
            else
            {
                string sectionName = "General";

                if (action.bindings[0].effectivePath.Contains("/"))
                    sectionName = action.bindings[0].effectivePath[..action.bindings[0].effectivePath.IndexOf('/')].Replace("<", "").Replace(">", "");
                else if (action.bindings[0].effectivePath == "Dpad")
                    sectionName = "Gamepad";

                ConfigEntry<string> actionConfig = Config.Bind(sectionName, action.name, action.bindings[0].effectivePath, $"Change binding for {action.name} ({sectionName}).\nThis is an action with a single binding");
                simpleConfigList.Add(actionConfig);
            }   
        });
    }

    //Don't create config items for these actions
    internal static List<string> invalidActions =
        [
            "AnyKey",
            "OpenDebugMenu",
            "TrackedDevicePosition",
            "TrackedDeviceOrientation",
        ];

    //from KeyBindValues.txt
    //Valid potential config values
    internal static List<string> ValidValues = 
        [
        "<Keyboard>/anyKey",
        "<Keyboard>/escape",
        "<Keyboard>/space",
        "<Keyboard>/enter",
        "<Keyboard>/tab",
        "<Keyboard>/backquote",
        "<Keyboard>/quote",
        "<Keyboard>/semicolon",
        "<Keyboard>/comma",
        "<Keyboard>/period",
        "<Keyboard>/slash",
        "<Keyboard>/backslash",
        "<Keyboard>/leftBracket",
        "<Keyboard>/rightBracket",
        "<Keyboard>/minus",
        "<Keyboard>/equals",
        "<Keyboard>/upArrow",
        "<Keyboard>/downArrow",
        "<Keyboard>/leftArrow",
        "<Keyboard>/rightArrow",
        "<Keyboard>/a",
        "<Keyboard>/b",
        "<Keyboard>/c",
        "<Keyboard>/d",
        "<Keyboard>/e",
        "<Keyboard>/f",
        "<Keyboard>/g",
        "<Keyboard>/h",
        "<Keyboard>/i",
        "<Keyboard>/j",
        "<Keyboard>/k",
        "<Keyboard>/l",
        "<Keyboard>/m",
        "<Keyboard>/n",
        "<Keyboard>/o",
        "<Keyboard>/p",
        "<Keyboard>/q",
        "<Keyboard>/r",
        "<Keyboard>/s",
        "<Keyboard>/t",
        "<Keyboard>/u",
        "<Keyboard>/v",
        "<Keyboard>/w",
        "<Keyboard>/x",
        "<Keyboard>/y",
        "<Keyboard>/z",
        "<Keyboard>/1",
        "<Keyboard>/2",
        "<Keyboard>/3",
        "<Keyboard>/4",
        "<Keyboard>/5",
        "<Keyboard>/6",
        "<Keyboard>/7",
        "<Keyboard>/8",
        "<Keyboard>/9",
        "<Keyboard>/0",
        "<Keyboard>/leftShift",
        "<Keyboard>/rightShift",
        "<Keyboard>/shift",
        "<Keyboard>/leftAlt",
        "<Keyboard>/rightAlt",
        "<Keyboard>/alt",
        "<Keyboard>/leftCtrl",
        "<Keyboard>/rightCtrl",
        "<Keyboard>/ctrl",
        "<Keyboard>/leftMeta",
        "<Keyboard>/rightMeta",
        "<Keyboard>/contextMenu",
        "<Keyboard>/backspace",
        "<Keyboard>/pageDown",
        "<Keyboard>/pageUp",
        "<Keyboard>/home",
        "<Keyboard>/end",
        "<Keyboard>/insert",
        "<Keyboard>/delete",
        "<Keyboard>/capsLock",
        "<Keyboard>/numLock",
        "<Keyboard>/printScreen",
        "<Keyboard>/scrollLock",
        "<Keyboard>/pause",
        "<Keyboard>/numpadEnter",
        "<Keyboard>/numpadDivide",
        "<Keyboard>/numpadMultiply",
        "<Keyboard>/numpadPlus",
        "<Keyboard>/numpadMinus",
        "<Keyboard>/numpadPeriod",
        "<Keyboard>/numpadEquals",
        "<Keyboard>/numpad1",
        "<Keyboard>/numpad2",
        "<Keyboard>/numpad3",
        "<Keyboard>/numpad4",
        "<Keyboard>/numpad5",
        "<Keyboard>/numpad6",
        "<Keyboard>/numpad7",
        "<Keyboard>/numpad8",
        "<Keyboard>/numpad9",
        "<Keyboard>/numpad0",
        "<Keyboard>/f1",
        "<Keyboard>/f2",
        "<Keyboard>/f3",
        "<Keyboard>/f4",
        "<Keyboard>/f5",
        "<Keyboard>/f6",
        "<Keyboard>/f7",
        "<Keyboard>/f8",
        "<Keyboard>/f9",
        "<Keyboard>/f10",
        "<Keyboard>/f11",
        "<Keyboard>/f12",
        "<Keyboard>/OEM1",
        "<Keyboard>/OEM2",
        "<Keyboard>/OEM3",
        "<Keyboard>/OEM4",
        "<Keyboard>/OEM5",
        "<Keyboard>/IMESelected",
        "<Mouse>/position",
        "<Mouse>/delta",
        "<Mouse>/scroll",
        "<Mouse>/press",
        "<Mouse>/leftButton",
        "<Mouse>/rightButton",
        "<Mouse>/middleButton",
        "<Mouse>/forwardButton",
        "<Mouse>/backButton",
        "<Mouse>/pressure",
        "<Mouse>/radius",
        "<Mouse>/pointerId",
        "<Mouse>/displayIndex",
        "<Mouse>/clickCount",
        "<Mouse>/position/x",
        "<Mouse>/position/y",
        "<Mouse>/delta/up",
        "<Mouse>/delta/down",
        "<Mouse>/delta/left",
        "<Mouse>/delta/right",
        "<Mouse>/delta/x",
        "<Mouse>/delta/y",
        "<Mouse>/scroll/up",
        "<Mouse>/scroll/down",
        "<Mouse>/scroll/left",
        "<Mouse>/scroll/right",
        "<Mouse>/scroll/x",
        "<Mouse>/scroll/y",
        "<Mouse>/radius/x",
        "<Mouse>/radius/y",
        "<Pen>/position",
        "<Pen>/delta",
        "<Pen>/tilt",
        "<Pen>/pressure",
        "<Pen>/twist",
        "<Pen>/tip",
        "<Pen>/press",
        "<Pen>/eraser",
        "<Pen>/inRange",
        "<Pen>/barrel1",
        "<Pen>/barrel2",
        "<Pen>/barrel3",
        "<Pen>/barrel4",
        "<Pen>/radius",
        "<Pen>/pointerId",
        "<Pen>/displayIndex",
        "<Pen>/position/x",
        "<Pen>/position/y",
        "<Pen>/delta/up",
        "<Pen>/delta/down",
        "<Pen>/delta/left",
        "<Pen>/delta/right",
        "<Pen>/delta/x",
        "<Pen>/delta/y",
        "<Pen>/tilt/x",
        "<Pen>/tilt/y",
        "<Pen>/radius/x",
        "<Pen>/radius/y",
        "<Gamepad>/dpad",
        "<Gamepad>/start",
        "<Gamepad>/select",
        "<Gamepad>/leftStickPress",
        "<Gamepad>/rightStickPress",
        "<Gamepad>/leftShoulder",
        "<Gamepad>/rightShoulder",
        "<Gamepad>/buttonSouth",
        "<Gamepad>/buttonEast",
        "<Gamepad>/buttonWest",
        "<Gamepad>/buttonNorth",
        "<Gamepad>/leftTrigger",
        "<Gamepad>/rightTrigger",
        "<Gamepad>/leftStick",
        "<Gamepad>/rightStick",
        "<Gamepad>/dpad/x",
        "<Gamepad>/dpad/y",
        "<Gamepad>/dpad/up",
        "<Gamepad>/dpad/down",
        "<Gamepad>/dpad/left",
        "<Gamepad>/dpad/right",
        "<Gamepad>/leftStick/up",
        "<Gamepad>/leftStick/x",
        "<Gamepad>/leftStick/y",
        "<Gamepad>/leftStick/down",
        "<Gamepad>/leftStick/left",
        "<Gamepad>/leftStick/right",
        "<Gamepad>/rightStick/up",
        "<Gamepad>/rightStick/x",
        "<Gamepad>/rightStick/y",
        "<Gamepad>/rightStick/down",
        "<Gamepad>/rightStick/left",
        "<Gamepad>/rightStick/right",
        "Dpad" //Added this one since it's used by base-game
        ];
}
