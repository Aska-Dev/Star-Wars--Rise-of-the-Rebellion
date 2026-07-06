using Godot;
using System;

public partial class AsteroidStormParticleEmitter : GpuParticles2D
{
	public override void _Ready()
	{
        EventHub.Instance.OnParticleEmitterStateChanged += OnParticleEmitterStateChanged;
    }

    public override void _ExitTree()
    {
        EventHub.Instance.OnParticleEmitterStateChanged -= OnParticleEmitterStateChanged;
    }

    private void OnParticleEmitterStateChanged((ParticleEmitter emitter, bool isActive) data)
    {
        if (data.emitter == ParticleEmitter.AsteroidStorm)
        {
            Emitting = data.isActive;
        }
    }
}
