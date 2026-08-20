using AdminToys;
using EffectParticles;
using EffectParticles.Features;
using LabApi.Features.Wrappers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParticleController : MonoBehaviour
{
    protected List<GameObject> _pool;
    private ParticleState[] _states;
    private int _cursor;
    private float _flightDuration;
    private float ArcHeight => Plugin.Singleton.Config.ArcHeight;
    private Player _player;

    public static List<ParticleController> AllParticleManagers = [];

    private struct ParticleState
    {
        public Transform Transform;
        public Vector3 InitScale;
        public Vector3 Start;
        public Vector3 End;
        public float Timer;
        public bool Active;
    }

    protected abstract GameObject CreateParticle();
    internal abstract void Recolor(Color color);

    internal void Initialize(int amount, Color color, float flightDuration, Player player)
    {
        _pool = CreateParticles(amount);
        Recolor(color);

        EffectsManager.RebuildParticles();

        _flightDuration = flightDuration;
        _player = player;

        _states = new ParticleState[_pool.Count];
        for (int i = 0; i < _pool.Count; i++)
        {
            var go = _pool[i];

            _states[i] = new ParticleState
            {
                InitScale = go.transform.localScale,
                Transform = go.transform,
                Active = false
            };

            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.zero;
        }
        _cursor = 0;

        AllParticleManagers.Add(this);
    }

    internal void LaunchParticle(Vector3 startPos, Vector3 endPos, Color color)
    {
        Recolor(color);

        int idx = _cursor;
        _cursor = (_cursor + 1) % _pool.Count;

        if (_states[idx].Active)
        {
            _cursor = idx;
            return;
        }

        var p = _states[idx];

        p.Transform.localPosition = Vector3.zero;
        p.Transform.localScale = p.InitScale;

        p.Start = startPos;
        p.End = endPos;
        p.Timer = 0f;
        p.Active = true;

        _states[idx] = p;
    }

    internal static void HideAllGameObjectsForPlayer(Player player)
    {
        foreach (var manager in AllParticleManagers)
        {
            manager.HideGameObjectsForPlayer(player);
        }
    }

    internal static void ShowAllGameObjectsForPlayer(Player player)
    {
        foreach (var manager in AllParticleManagers)
        {
            manager.ShowGameObjectsForPlayer(player);
        }
    }

    internal void HideGameObjectsForPlayer(Player player)
    {
        foreach (var gameObject in _pool) 
        {
            if (!gameObject.TryGetComponent<NetworkBehaviour>(out var networkBehaviour))
            {
                Logger.Warn($"{gameObject.name} doesn't have a NetworkBehaviour component");
                continue;
            }

            var destroyMessage = new ObjectDestroyMessage
            {
                netId = networkBehaviour.netId
            };

            player.Connection.Send(destroyMessage);
        }
    }

    internal void ShowGameObjectsForPlayer(Player player)
    {
        foreach (var gameObject in _pool)
        {
            if (!gameObject.TryGetComponent<NetworkBehaviour>(out var networkBehaviour))
            {
                Logger.Warn($"{gameObject.name} doesn't have a NetworkBehaviour component");
                continue;
            }

            NetworkServer.SendSpawnMessage(networkBehaviour.netIdentity, player.Connection);
        }
    }

    private List<GameObject> CreateParticles(int amount)
    {
        List<GameObject> list = [];
        for (int i = 0; i < amount; i++)
        {
            list.Add(CreateParticle());
        }

        return list;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < _states.Length; i++)
        {
            var p = _states[i];
            if (!p.Active) continue;

            p.Timer += dt;
            float t = p.Timer / _flightDuration;

            if (t >= 1f)
            {
                p.Transform.localPosition = Vector3.zero;
                p.Transform.localScale = Vector3.zero;
                p.Timer = 0f;
                p.Active = false;
                t = 0f;
            }

            float delta = p.End.y - p.Start.y;
            Vector3 control = new Vector3(
                (p.Start.x + p.End.x) * 0.5f,
                Mathf.Max(p.Start.y, p.End.y) + ArcHeight + Mathf.Abs(delta) * 0.25f,
                (p.Start.z + p.End.z) * 0.5f
            );

            float mt = 1f - t;
            Vector3 pos = mt * mt * p.Start + 2f * mt * t * control + t * t * p.End;
            p.Transform.localPosition = pos;

            float scale = 1f - t * t;
            if (scale < 0.9f)
            {
                p.Transform.localScale = p.InitScale * Mathf.Max(0f, scale);
            }

            _states[i] = p;
        }
    }

    private void OnDestroy()
    {
        AllParticleManagers.Remove(this);

        foreach (var gameObject in _pool)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}