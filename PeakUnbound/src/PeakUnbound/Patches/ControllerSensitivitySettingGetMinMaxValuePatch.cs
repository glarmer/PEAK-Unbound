using HarmonyLib;
using Unity.Mathematics;

namespace PEAKUnbound.Patches;

public class ControllerSensitivitySettingGetMinMaxValuePatch
{
    [HarmonyPatch(typeof(ControllerSensitivitySetting), "GetMinMaxValue")]
    [HarmonyPrefix]
    static bool Prefix(ref float2 __result)
    {
        __result = new float2(0.1f, 20f);
        return false;
    }
}