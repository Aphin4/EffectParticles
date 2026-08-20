using CustomPlayerEffects;
using EffectParticles.Enums;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EffectParticles.Features;

internal static class EffectsManager
{
    internal static readonly List<Player> ActivePlayers = [];
    internal static CoroutineHandle MainCoroutine;
    private static Config Config => Plugin.Singleton.Config;
    private readonly static Type _particlesType = Config.ParticlesType;

    internal static IEnumerator<float> UpdatePlayerEffects()
    {
        float timeBeforeNext = 1 / Config.ParticlesPerSecond;
        int poolAmount = Convert.ToInt32(MathF.Ceiling(Config.Lifetime * Config.ParticlesPerSecond));

        while (ActivePlayers.Count > 0)
        {
            foreach (Player player in ActivePlayers.ToList())
            {
                if (player == null)
                {
                    continue;
                }

                if (player.ActiveEffects.IsEmpty())
                {
                    ActivePlayers.Remove(player);
                    continue;
                }

                Color? finalColor = null;

                foreach (StatusEffectBase effect in player.ActiveEffects)
                {
                    Color effectColor = GetColor(effect);

                    if (finalColor == null)
                    {
                        if (effectColor == null)
                            finalColor = Color.white;
                        else 
                            finalColor = effectColor;
                    }
                    else
                        finalColor = Color.Lerp(finalColor.Value, effectColor, 0.5f);
                }

                if (!player.GameObject.TryGetComponent<ParticleController>(out ParticleController particleManager))
                {
                    var notCastedManager = player.GameObject.AddComponent(_particlesType);
                    ParticleController manager = (ParticleController)notCastedManager;
                    manager.Initialize(poolAmount, finalColor.Value, Config.Lifetime, player);

                    particleManager = manager;
                }

                Vector3 startPos = new(0, UnityEngine.Random.Range(-0.1f, 0.45f), 0);

                float xminus = UnityEngine.Random.Range(-0.2f, -0.5f);
                float xplus = UnityEngine.Random.Range(0.2f, 0.5f);
                float xpos = Mathf.Abs(xminus) > Mathf.Abs(xplus) ? xminus : xplus;

                float zminus = UnityEngine.Random.Range(-0.2f, -0.5f);
                float zplus = UnityEngine.Random.Range(0.2f, 0.5f);
                float zpos = Mathf.Abs(zminus) > Mathf.Abs(zplus) ? zminus : zplus;

                Vector3 finalPos = new(xpos, UnityEngine.Random.Range(-0.1f, 0.3f), zpos);

                particleManager.LaunchParticle(startPos, finalPos, finalColor.Value);
            }
            yield return Timing.WaitForSeconds(timeBeforeNext);
        }
    }

    internal static void RebuildParticles()
    {
        foreach (Player player in Player.List)
        {
            if (!player.IsPlayer)
                continue;

            if (SSParticleVisible.PlayerSettings.TryGetValue(player, out SettingsState settings))
            {
                bool hasController = player.GameObject.TryGetComponent<ParticleController>(out var component);

                switch (settings)
                {
                    case (SettingsState.None):
                    case (SettingsState.OwnParticles):
                        {
                            ParticleController.HideAllGameObjectsForPlayer(player);
                            break;
                        }
                    case (SettingsState.AllParticles):
                        {
                            ParticleController.ShowAllGameObjectsForPlayer(player);
                            if (hasController)
                                component.HideGameObjectsForPlayer(player);
                            break;
                        }
                    case (SettingsState.AllAndOwnParticles):
                        {
                            ParticleController.ShowAllGameObjectsForPlayer(player);
                            break;
                        }
                }
            }
            else
            {
                Logger.Error("Error while receiving last " + player.Nickname + " settings state");
            }
        }
    }

    private static Color GetColor(StatusEffectBase effect)
    {
        Type realType = effect.GetType();

        if (Plugin.Singleton.Config.EffectColors.TryGetValue(realType, out string hexColor))
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                return color;
            }
            else GetColorByClassification(effect);
        }

        return GetColorByClassification(effect);
    }

    private static Color GetColorByClassification(StatusEffectBase statusEffectBase)
    {
        return statusEffectBase.Classification switch
        {
            StatusEffectBase.EffectClassification.Technical => new Color(0.82f, 0.82f, 0.28f, 0.5f),
            StatusEffectBase.EffectClassification.Negative => new Color(0.82f, 0.28f, 0.28f, 0.5f),
            StatusEffectBase.EffectClassification.Positive => new Color(0.28f, 0.82f, 0.28f, 0.5f),
            StatusEffectBase.EffectClassification.Mixed => new Color(0.82f, 0.28f, 0.82f, 0.5f),
            _ => Color.white,
        };
    }
}