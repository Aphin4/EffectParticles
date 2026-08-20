global using Logger = LabApi.Features.Console.Logger;
using EffectParticles.Features;
using EffectParticles.Handlers;
using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using System;

namespace EffectParticles;

public class Plugin : Plugin<Config>
{
    public override string Name { get; } = "EffectParticles";
    public override string Description { get; } = "";
    public override string Author => "Aphin";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion => new(1, 1, 7);
    public static Plugin Singleton;
    private Harmony _patch = new("com.aphin.effectparticles");

    public override void Enable()
    {
        Singleton = this;

        _patch.PatchAll();

        SSParticleVisible.CreateSSSettings();
        InternalHandler.Init();
    }
    public override void Disable()
    {
        _patch.UnpatchAll("com.aphin.effectparticles");

        InternalHandler.Dispose();

        Singleton = null;
    }
}