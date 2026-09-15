using System.Collections.Generic;
using UnityEngine;

public class TriggerParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleOnTrigger;
    [SerializeField]
    private Vector3 _particleOffset;
    
    private ParticleSystem _thisPs;
    private List<ParticleSystem.Particle> _enterParticles = new List<ParticleSystem.Particle>();

    private void Awake()
    {
        _thisPs = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        if(!_particleOnTrigger)
        {
            return;
        }
        int count = _thisPs.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, _enterParticles);
        if(count > 0)
        {
            _particleOnTrigger.transform.position = _enterParticles[0].position + _particleOffset;
            _particleOnTrigger.Clear(true);
            _particleOnTrigger.Play(true);
        }

    }
}
