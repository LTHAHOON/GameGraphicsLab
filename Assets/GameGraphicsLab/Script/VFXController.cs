using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField]
    private float _baseSpinTorque = 10f;
    [SerializeField]
    private Rigidbody _dinnerWheelRigid;
    [SerializeField]
    private VisualEffect _vfx;
    [SerializeField]
    private ParticleSystem _fireworkPs;

    private bool _onClick = false;
    private bool _hasStartedSpinning = false;
    private void Awake()
    {
        _vfx.Reinit();
    }

    private void FixedUpdate()
    {
        if (!_onClick)
        {
            return;
        }

        float angularSpeed = _dinnerWheelRigid.angularVelocity.sqrMagnitude;
        if (!_hasStartedSpinning)
        {
            _hasStartedSpinning = angularSpeed > 0.1f;
            return;
        }

        if (angularSpeed <= 0.1f)
        {
            _fireworkPs.Play();
            _onClick = false;
            _hasStartedSpinning = false;
        }
    }

    public void OnFire(InputValue inputValue)
    {
        if(!inputValue.isPressed)
        {
            return;
        }

        _vfx.Reinit();
        float randomSpeed = Random.Range(_baseSpinTorque - 7f, _baseSpinTorque);
        _dinnerWheelRigid.AddTorque(_dinnerWheelRigid.transform.forward * randomSpeed, ForceMode.Impulse);
        _vfx.Play();
        Debug.Log("Fire");
        _onClick = true;
        _hasStartedSpinning = false;
    }
}
