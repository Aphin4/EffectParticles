using CustomPlayerEffects;
using EffectParticles.Events;
using HarmonyLib;

namespace EffectParticles.Patches;

[HarmonyPatch(typeof(StatusEffectBase), "Intensity", MethodType.Setter)]
public static class IntensitySetterPatch
{
    [HarmonyPrefix]
    public static void Prefix(StatusEffectBase __instance, out bool __state)
    {
        __state = __instance.IsEnabled;
    }

    [HarmonyPostfix]
    public static void Postfix(StatusEffectBase __instance, byte value, bool __state)
    {
        bool previousIsEnabled = __state;
        bool currentIsEnabled = value > 0;
        if (currentIsEnabled != previousIsEnabled)
        {
            EffectEvents.OnEnabledChanged(__instance, currentIsEnabled);
        }
    }
}
