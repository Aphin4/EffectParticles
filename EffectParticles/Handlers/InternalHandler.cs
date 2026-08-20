using CustomPlayerEffects;
using EffectParticles.Events;
using EffectParticles.Features;
using EffectParticles.Features.Particles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using System;
using UserSettings.ServerSpecific;

namespace EffectParticles.Handlers;

internal static class InternalHandler
{
    internal static void Init()
    {
        EffectEvents.EnabledChanged += OnEffectEnabled;

        PlayerEvents.Left += OnPlayerLeft;
        PlayerEvents.Joined += OnPlayerJoined;
        ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnServerOnSettingValueReceived;

        ServerEvents.WaitingForPlayers += ConfigExtensions.Init;
    }
    internal static void Dispose()
    {
        EffectEvents.EnabledChanged -= OnEffectEnabled;

        PlayerEvents.Left -= OnPlayerLeft;
        PlayerEvents.Joined -= OnPlayerJoined;
        ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnServerOnSettingValueReceived;

        ServerEvents.WaitingForPlayers -= ConfigExtensions.Init;
    }

    private static void OnEffectEnabled(StatusEffectBase effect, bool enabled)
    {
        Player player = Player.Get(effect.Hub);

        if (player == null)
        {
            Logger.Error($"Player is null for effect {effect.GetType().Name}");
            return;
        }

        if (enabled)
        {
            if (!EffectsManager.ActivePlayers.Contains(player))
            {
                EffectsManager.ActivePlayers.Add(player);
            }
            if (!EffectsManager.MainCoroutine.IsRunning)
            {
                EffectsManager.MainCoroutine = Timing.RunCoroutine(EffectsManager.UpdatePlayerEffects());
            }
        }
    }

    private static void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        if (EffectsManager.ActivePlayers.Contains(ev.Player))
            EffectsManager.ActivePlayers.Remove(ev.Player);

        if (ev.Player.GameObject.TryGetComponent<ParticleController>(out var manager))
        {
            UnityEngine.Object.Destroy(manager);
        }
    }

    private static void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        Timing.CallDelayed(1f, () => SSParticleVisible.CollectSettings(ev.Player));
    }

    private static void OnServerOnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingsBase)
    {
        if (settingsBase.SettingId == SSParticleVisible.StaticOwnParticlesId || settingsBase.SettingId == SSParticleVisible.StaticAllParticlesId)
        {
            SSParticleVisible.CollectSettings(Player.Get(hub));
        }
    }
}
