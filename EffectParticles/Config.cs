using EffectParticles.Features.Particles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EffectParticles;

public class Config
{
    [Description("Text for the Enabled setting")]
    public string SSSettingEnabledString { get; set; } = "On";
    [Description("Text for the Disabled setting")]
    public string SSSettingDisabledString { get; set; } = "Off";
    [Description("Text for the setting responsible for the own particles")]
    public string SSSettingOwnParticlesString { get; set; } = "Show own particles";
    [Description("Text for the setting responsible for the all particles")]
    public string SSSettingAllParticlesString { get; set; } = "Show all particles";
    [Description("Colors for each effect, use only the original names of the effect classes")]
    public Dictionary<string, string> Colors { get; set; } = new()
    {
        ["Scp1853"] = "#FF00FF80",          
        ["Invigorated"] = "#00FF0080",           
        ["RainbowTaste"] = "#FF69B480",     
        ["BodyshotReduction"] = "#FFA50080",
        ["DamageReduction"] = "#FFD70080",  
        ["MovementBoost"] = "#00FFFF80",    
        ["Vitality"] = "#FF149380",         
        ["SpawnProtected"] = "#FFFFFF80",  
        ["Ghostly"] = "#B0C4DE80",          
        ["SilentWalk"] = "#69696980",       
        ["Fade"] = "#2F4F4F80",             
        ["FocusedVision"] = "#0000FF80",    
        ["AnomalousRegeneration"] = "#7FFF0080",
        ["Scp1344"] = "#DEB88780",
        ["Scp207"] = "#FF000080",           
        ["AntiScp207"] = "#4B008280",       
        ["AmnesiaVision"] = "#9370DB80",
        ["AmnesiaItems"] = "#80808080",     
        ["Asphyxiated"] = "#19197080",      
        ["Bleeding"] = "#8B000080",       
        ["Blurred"] = "#77889980",          
        ["Burned"] = "#FF450080",           
        ["Concussed"] = "#FF8C0080",        
        ["Corroding"] = "#ADFF2F80",      
        ["PocketCorroding"] = "#00640080",
        ["Deafened"] = "#A9A9A980",       
        ["Decontaminating"] = "#00CED180",  
        ["Disabled"] = "#80000080",         
        ["Ensnared"] = "#8B451380",
        ["Exhausted"] = "#483D8B80",
        ["Flashed"] = "#FFFF0080",
        ["Hemorrhage"] = "#4A000080",
        ["Hypothermia"] = "#ADD8E680",
        ["Poisoned"] = "#00800080",         
        ["Sinkhole"] = "#3E272380",
        ["Stained"] = "#A0522D80",          
        ["SeveredHands"] = "#CD5C5C80",
        ["Traumatized"] = "#2E085480",
        ["CardiacArrest"] = "#B2222280",
        ["Strangled"] = "#80008080",        
        ["Slowness"] = "#6B8E2380",         
        ["Blindness"] = "#00000080",        
        ["SeveredEyes"] = "#A52A2A80",      
        ["PitDeath"] = "#00330080",         
        ["AnomalousTarget"] = "#FF333380",  
        ["SoundtrackMute"] = "#55555580",
        ["Scanned"] = "#40E0D080",
        ["FogControl"] = "#D3D3D380",
        ["Scp1576"] = "#C7158580",         
        ["Lightweight"] = "#FFFFE080",
        ["HeavyFooted"] = "#5C403380",  
        ["Scp1509Resurrected"] = "#D2691E80",
        ["NightVision"] = "#39FF1480"
    };
    [Description("Lifetime for every particle in seconds")]
    public float Lifetime { get; set; } = 2f;
    [Description("How many particles spawning per second")]
    public float ParticlesPerSecond { get; set; } = 3;
    [Description("Modifier for the particle motion curve")]
    public float ArcHeight { get; set; } = 0.2f;
    [Description("Type of particles")]
    public string TypeParticles { get; set; } = "SphereParticles";
    internal Type ParticlesType;
    internal Dictionary<Type, string> EffectColors;
}

internal static class ConfigExtensions
{
    private static Config Config => Plugin.Singleton.Config;

    internal static void Init()
    {
        Config.EffectColors = GetEffectTypes();

        Type particlesType = FindTypeByName(Config.TypeParticles);
        if (!typeof(ParticleController).IsAssignableFrom(particlesType) || particlesType.IsAbstract)
        {
            Config.ParticlesType = typeof(SphereParticles);
            Logger.Warn($"The {particlesType.Name} doesn`t inherit from ParticleController");
        }
        else 
            Config.ParticlesType = particlesType;
    }

    private static Dictionary<Type, string> GetEffectTypes()
    {
        Dictionary<Type, string> dict = [];

        foreach (var pair in Plugin.Singleton.Config.Colors)
        {
            Type type = FindTypeByName(pair.Key);

            if (type != null)
            {
                dict[type] = pair.Value;
            }
            else
            {
                Logger.Warn($"Type '{pair.Key}' not found");
            }
        }

        return dict;
    }

    private static Type FindTypeByName(string typeName)
    {
        Type type = Type.GetType(typeName);
        if (type != null) return type;

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try { return assembly.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);
    }

}
