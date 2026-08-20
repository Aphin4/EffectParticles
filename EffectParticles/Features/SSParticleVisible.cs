using EffectParticles.Enums;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;

namespace EffectParticles.Features;

internal static class SSParticleVisible
{
    internal static int StaticOwnParticlesId = 91285;
    internal static int StaticAllParticlesId = 91125;
    internal static int StaticLabelId;
    internal static Dictionary<Player, SettingsState> PlayerSettings = [];

    private static Config Config => Plugin.Singleton.Config;

    internal static void CreateSSSettings()
    {
        List<ServerSpecificSettingBase> settings = ServerSpecificSettingsSync.DefinedSettings?.ToList() ?? [];

        SSGroupHeader header = new(StaticLabelId, "Effect Particles");
        settings.Add(header);

        SSTwoButtonsSetting ownParticlesSetting = new(StaticOwnParticlesId, Config.SSSettingOwnParticlesString, Config.SSSettingEnabledString, Config.SSSettingDisabledString, true);
        settings.Add(ownParticlesSetting);

        SSTwoButtonsSetting allParticlesSetting = new(StaticAllParticlesId, Config.SSSettingAllParticlesString, Config.SSSettingEnabledString, Config.SSSettingDisabledString);
        settings.Add(allParticlesSetting);

        ServerSpecificSettingsSync.DefinedSettings = [.. settings];

        ServerSpecificSettingsSync.SendToAll();
    }

    internal static void CollectSettings(Player player)
    {
        bool own = false;
        bool all = false;

        if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSTwoButtonsSetting>(player.ReferenceHub, StaticOwnParticlesId, out SSTwoButtonsSetting ownParticles))
        {
            if (ownParticles.SyncIsA)
                own = true;
        }
        if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSTwoButtonsSetting>(player.ReferenceHub, StaticAllParticlesId, out SSTwoButtonsSetting allParticles))
        {
            if (allParticles.SyncIsA)
                all = true;
        }

        if (own && all)
            PlayerSettings[player] = SettingsState.AllAndOwnParticles;
        else if (own)
            PlayerSettings[player] = SettingsState.OwnParticles;
        else if (all)
            PlayerSettings[player] = SettingsState.AllParticles;
        else
            PlayerSettings[player] = SettingsState.None;

        EffectsManager.RebuildParticles();
    }
}
