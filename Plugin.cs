using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PEAKUnbound;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private static ConfigEntry<string> configPause;
    private static ConfigEntry<string> configMove;
    private static ConfigEntry<string> configLook;
    private static ConfigEntry<string> configJump;
    private static ConfigEntry<string> configSprint;
    private static ConfigEntry<string> configSprintToggle;
    private static ConfigEntry<string> configInteract;
    private static ConfigEntry<string> configDrop;
    private static ConfigEntry<string> configCrouch;
    private static ConfigEntry<string> configCrouchToggle;
    private static ConfigEntry<string> configUsePrimary;
    private static ConfigEntry<string> configUseSecondary;
    private static ConfigEntry<string> configScroll;
    private static ConfigEntry<string> configPushToTalk;
    private static ConfigEntry<string> configEmote;
    private static ConfigEntry<string> configPing;
    private static ConfigEntry<string> configSelectSlotForward;
    private static ConfigEntry<string> configSelectSlotBackward;
    private static ConfigEntry<string> configSpectateLeft;
    private static ConfigEntry<string> configSpectateRight;
    private static ConfigEntry<string> configScrollButtonLeft;
    private static ConfigEntry<string> configScrollButtonRight;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        configPause = Config.Bind
        (
            "General", 
            "Pause", 
            "<Keyboard>/escape", 
            "Pause Button"
        );
        InputSystem.actions.FindAction("Pause", false).ChangeBinding(0).WithPath(configPause.Value);
        CharacterInput.action_pause = InputSystem.actions.FindAction("Pause", false);
        
        //configMove = Config.Bind
        //(
        //    "General", 
        //    "Move", 
        //    "w,a,s,d", 
        //    "Move Buttons"
        //);
        //string forward = configMove.Value.Split(',')[0];
        //string backwards = configMove.Value.Split(',')[2];
        //string left = configMove.Value.Split(',')[1];
        //string right = configMove.Value.Split(',')[3];
        
        //InputSystem.actions.FindAction("Move", false).ChangeBinding(1).WithPath("<Keyboard>/"+forward);
        //InputSystem.actions.FindAction("Move", false).ChangeBinding(3).WithPath("<Keyboard>/"+backwards);
        //InputSystem.actions.FindAction("Move", false).ChangeBinding(5).WithPath("<Keyboard>/"+left);
        //InputSystem.actions.FindAction("Move", false).ChangeBinding(7).WithPath("<Keyboard>/"+right);
        //CharacterInput.action_move = InputSystem.actions.FindAction("Move", false);
        
        //InputSystem.actions.FindAction("Look", false).;
        //CharacterInput.action_pause = InputSystem.actions.FindAction("Look", false);
        
        configJump = Config.Bind
        (
            "General", 
            "Jump", 
            "<Keyboard>/space", 
            "Jump Button"
        );
        InputSystem.actions.FindAction("Jump", false).ChangeBinding(0).WithPath(configJump.Value);
        CharacterInput.action_jump = InputSystem.actions.FindAction("Jump", false);
        
        configSprint = Config.Bind
        (
            "General", 
            "Sprint", 
            "<Keyboard>/leftShift", 
            "Sprint Button"
        );
        InputSystem.actions.FindAction("Sprint", false).ChangeBinding(0).WithPath(configSprint.Value);
        CharacterInput.action_sprint = InputSystem.actions.FindAction("Sprint", false);
        
        configInteract = Config.Bind
        (
            "General", 
            "Interact", 
            "<Keyboard>/e", 
            "Interact Button"
        );
        InputSystem.actions.FindAction("Interact", false).ChangeBinding(0).WithPath(configInteract.Value);
        CharacterInput.action_interact = InputSystem.actions.FindAction("Interact", false);
        
        configDrop = Config.Bind
        (
            "General", 
            "Drop", 
            "<Keyboard>/q", 
            "Drop Button"
        );
        InputSystem.actions.FindAction("Drop", false).ChangeBinding(0).WithPath(configDrop.Value);
        CharacterInput.action_drop = InputSystem.actions.FindAction("Drop", false);
        
        configCrouch = Config.Bind
        (
            "General", 
            "Crouch", 
            "<Keyboard>/ctrl", 
            "Crouch Button"
        );
        InputSystem.actions.FindAction("Crouch", false).ChangeBinding(0).WithPath(configCrouch.Value);
        CharacterInput.action_crouch = InputSystem.actions.FindAction("Crouch", false);
        
        configUsePrimary = Config.Bind
        (
            "General", 
            "Use Primary", 
            "<Mouse>/leftButton", 
            "Use Primary Button"
        );
        InputSystem.actions.FindAction("UsePrimary", false).ChangeBinding(0).WithPath(configUsePrimary.Value);
        CharacterInput.action_usePrimary = InputSystem.actions.FindAction("UsePrimary", false);
        
        configUseSecondary = Config.Bind
        (
            "General", 
            "Use Secondary", 
            "<Mouse>/rightButton", 
            "Use Secondary Button"
        );
        InputSystem.actions.FindAction("UseSecondary", false).ChangeBinding(0).WithPath(configUseSecondary.Value);
        CharacterInput.action_useSecondary = InputSystem.actions.FindAction("UseSecondary", false);
        
        configPushToTalk = Config.Bind
        (
            "General", 
            "PushToTalk", 
            "<Keyboard>/v", 
            "PushToTalk Button"
        );
        InputSystem.actions.FindAction("PushToTalk", false).ChangeBinding(0).WithPath(configPushToTalk.Value);
        CharacterInput.push_to_talk = InputSystem.actions.FindAction("PushToTalk", false);
        
        configEmote = Config.Bind
        (
            "General", 
            "Emote", 
            "<Keyboard>/r", 
            "Emote Button"
        );
        InputSystem.actions.FindAction("Emote", false).ChangeBinding(0).WithPath(configEmote.Value);
        CharacterInput.action_emote = InputSystem.actions.FindAction("Emote", false);
        
        configPing = Config.Bind
        (
            "General", 
            "Ping", 
            "<Mouse>/middleButton", 
            "Ping Button"
        );
        InputSystem.actions.FindAction("Ping", false).ChangeBinding(0).WithPath(configPing.Value);
        CharacterInput.action_ping = InputSystem.actions.FindAction("Ping", false);
        
        configSpectateLeft = Config.Bind
        (
            "General", 
            "SpectateLeft", 
            "<Keyboard>/a", 
            "SpectateLeft Button"
        );
        InputSystem.actions.FindAction("SpectateLeft", false).ChangeBinding(0).WithPath(configSpectateLeft.Value);
        CharacterInput.action_spectateLeft = InputSystem.actions.FindAction("SpectateLeft", false);
        
        configSpectateRight = Config.Bind
        (
            "General", 
            "SpectateRight", 
            "<Keyboard>/d", 
            "SpectateRight Button"
        );
        InputSystem.actions.FindAction("SpectateRight", false).ChangeBinding(0).WithPath(configSpectateRight.Value);
        CharacterInput.action_spectateRight = InputSystem.actions.FindAction("SpectateRight", false);
    }
}
