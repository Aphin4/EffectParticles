using AdminToys;
using UnityEngine;
using PrimitiveWrapper = LabApi.Features.Wrappers.PrimitiveObjectToy;

namespace EffectParticles.Features.Particles;

internal class SphereParticles : ParticleController
{
    protected override GameObject CreateParticle()
    {
        float size = Random.Range(0.05f, 0.07f);
        var primitive = PrimitiveWrapper.Create(Vector3.zero, Quaternion.Euler(0, 0, 0), new Vector3(size, size, size));
        primitive.Flags = PrimitiveFlags.Visible;
        primitive.Type = PrimitiveType.Sphere;
        primitive.Base.syncInterval = 0.033f;

        return primitive.GameObject;
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
