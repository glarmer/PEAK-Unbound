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
    private static ConfigEntry<string> configForward;
    private static ConfigEntry<string> configBackward;
    private static ConfigEntry<string> configLeft;
    private static ConfigEntry<string> configRight;
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
    private static ConfigEntry<string> configScrollUp;
    private static ConfigEntry<string> configScrollDown;
    private static ConfigEntry<string> configPushToTalk;
    private static ConfigEntry<string> configEmote;
    private static ConfigEntry<string> configPing;
    private static ConfigEntry<string> configSelectSlotForward;
    private static ConfigEntry<string> configSelectSlotBackward;
    private static ConfigEntry<string> configSpectateLeft;
    private static ConfigEntry<string> configSpectateRight;
    private static ConfigEntry<string> configScrollButtonLeft;
    private static ConfigEntry<string> configScrollButtonRight;
    
    
    private static ConfigEntry<string> configHotBarAction1;
    private static ConfigEntry<string> configHotBarAction2;
    private static ConfigEntry<string> configHotBarAction3;
    
    private void RebindMoveKeys()
    {
        var moveAction = InputSystem.actions.FindAction("Move", false);
        if (moveAction == null)
        {
            Logger.LogInfo("Move action not found.");
            return;
        }

        for (int i = 0; i < moveAction.bindings.Count; i++)
        {
            var binding = moveAction.bindings[i];

            if (!binding.isPartOfComposite) continue;

            switch (binding.name)
            {
                case "up":
                    moveAction.ChangeBinding(i).WithPath(configForward.Value);
                    break;
                case "down":
                    moveAction.ChangeBinding(i).WithPath(configBackward.Value);
                    break;
                case "left":
                    moveAction.ChangeBinding(i).WithPath(configLeft.Value);
                    break;
                case "right":
                    moveAction.ChangeBinding(i).WithPath(configRight.Value);
                    break;
            }
        }
    }
    
    private void RebindScrollAction()
    {
        var scrollAction = InputSystem.actions.FindAction("Scroll", false);
        if (scrollAction == null)
        {
            Logger.LogInfo("Scroll action not found.");
            return;
        }

        for (int i = 0; i < scrollAction.bindings.Count; i++)
        {
            var binding = scrollAction.bindings[i];

            if (!binding.isPartOfComposite) continue;

            switch (binding.name)
            {
                case "up":
                    scrollAction.ChangeBinding(i).WithPath(configScrollUp.Value);
                    break;
                case "down":
                    scrollAction.ChangeBinding(i).WithPath(configScrollDown.Value);
                    break;
            }
        }
    }
    
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

        configForward = Config.Bind
        (
            "General", 
            "Forward", 
            "<Keyboard>/w", 
            "Forward Movement Button"
        );
        configBackward = Config.Bind
        (
            "General", 
            "Backward", 
            "<Keyboard>/s", 
            "Backward Movement Button"
        );
        configLeft = Config.Bind
        (
            "General", 
            "Left", 
            "<Keyboard>/a", 
            "Left Movement Button"
        );
        configRight = Config.Bind
        (
            "General", 
            "Right", 
            "<Keyboard>/d", 
            "Right Movement Button"
        );
        RebindMoveKeys();
        //configRight = Config.Bind
        //(
        //    "General", 
        //    "ScrollUp", 
        //    "<Mouse>/scroll/up", 
        //    "Scroll Up Button"
        //);
        //configRight = Config.Bind
        //(
        //    "General", 
        //    "ScrollDown", 
        //    "<Mouse>/scroll/down", 
        //    "Scroll Down Button"
        //);
        //RebindScrollAction();
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
        
        configHotBarAction1 = Config.Bind
        (
            "General", 
            "Hotbar1", 
            "<Keyboard>/1", 
            "Hotbar1 Button"
        );
        InputSystem.actions.FindAction("Hotbar1", false).ChangeBinding(0).WithPath(configHotBarAction1.Value);
        CharacterInput.hotbarActions[0] = InputSystem.actions.FindAction("Hotbar1", false);
        
        configHotBarAction2 = Config.Bind
        (
            "General", 
            "Hotbar2", 
            "<Keyboard>/2", 
            "Hotbar2 Button"
        );
        InputSystem.actions.FindAction("Hotbar2", false).ChangeBinding(0).WithPath(configHotBarAction2.Value);
        CharacterInput.hotbarActions[1] = InputSystem.actions.FindAction("Hotbar2", false);
        
        configHotBarAction3 = Config.Bind
        (
            "General", 
            "Hotbar3", 
            "<Keyboard>/3", 
            "Hotbar3 Button"
        );
        InputSystem.actions.FindAction("Hotbar3", false).ChangeBinding(0).WithPath(configHotBarAction3.Value);
        CharacterInput.hotbarActions[2] = InputSystem.actions.FindAction("Hotbar3", false);
    }
}
