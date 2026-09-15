using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class TriggerParticle : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem _particlePrefab;
    [SerializeField]
    private Vector3 _particleOffset;
    
    private ParticleSystem _thisPs;
    private List<ParticleSystem.Particle> _enterParticles = new List<ParticleSystem.Particle>();
    private ObjectPool<ParticleSystem> _psPool;

    private void Awake()
    {
        _psPool = new(CreateParticle, OnGetParticle, OnReleaseParticle, OnDestroyParticle, false, 5, 10);
        _thisPs = GetComponent<ParticleSystem>();
    }

    private ParticleSystem CreateParticle()
    {
        return Instantiate(_particlePrefab);
    }

    private void OnGetParticle(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
    }

    private void OnReleaseParticle(ParticleSystem ps)
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
    }

    private void OnDestroyParticle(ParticleSystem ps)
    {
        if (ps != null)
        {
            Destroy(ps.gameObject);
        }
    }

    private void OnParticleTrigger()
    {
        if(!_particlePrefab)
        {
            return;
        }
        int count = _thisPs.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, _enterParticles);
        if(count > 0)
        {
            OnPlayParticle(_enterParticles[0].position);
        }
    }

    private async void OnPlayParticle(Vector3 position)
    {
        ParticleSystem ps = _psPool.Get();
        ps.transform.position = position + _particleOffset;
        ps.Play(true);
        Debug.Log("Call");
        while (ps != null && ps.isPlaying)
        {
            await Task.Yield();
        }

        if (ps != null)
        {
            _psPool.Release(ps);
        }
    }
}
