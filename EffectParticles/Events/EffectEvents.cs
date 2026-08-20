using CustomPlayerEffects;
using System;

namespace EffectParticles.Events;

public static class EffectEvents
{
    public static event Action<StatusEffectBase, bool> EnabledChanged;
    public static void OnEnabledChanged(StatusEffectBase effect, bool enabled)
    {
        EnabledChanged?.Invoke(effect, enabled);
    }
}