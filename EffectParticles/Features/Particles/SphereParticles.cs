using System.Collections.Generic;
using AdminToys;
using PrimitiveWrapper = LabApi.Features.Wrappers.PrimitiveObjectToy;
using UnityEngine;

namespace EffectParticles.Features.Particles;

internal class SphereParticles : ParticleController
{
    protected override List<GameObject> CreateParticles(int amount)
    {
        List<GameObject> list = [];

        for (int i = 0; i < amount; i++)
        {
            float size = Random.Range(0.05f, 0.07f);

            var primitive = PrimitiveWrapper.Create(Vector3.zero, Quaternion.Euler(0, 0, 0), new Vector3(size, size, size));
            primitive.Flags = PrimitiveFlags.Visible;
            primitive.Type = PrimitiveType.Sphere;
            primitive.Base.syncInterval = 0.033f;
            list.Add(primitive.GameObject);
        }

        return list;
    }
    internal override void Recolor(Color color)
    {
        if (color == null)
            color = Color.white;

        foreach(var go in _pool)
        {
            if (go.TryGetComponent<PrimitiveObjectToy>(out PrimitiveObjectToy primitive))
            {
                primitive.NetworkMaterialColor = color;
            }
        }
    }
}
