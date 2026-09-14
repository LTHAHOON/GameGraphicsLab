using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _vfx;

    private void Awake()
    {
        _vfx.Reinit();
    }

    public void OnFire(InputValue inputValue)
    {
        if(!inputValue.isPressed)
        {
            return;
        }

       // _vfx.Reinit();
        _vfx.Play();
        Debug.Log("Fire");
    }
}
