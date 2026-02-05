using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    public static CameraShake Instance;
    private bool canShake = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
       
    }

    public void ScreenShake(float powerAmount)
    {
        if (canShake)
        {
            impulseSource.GenerateImpulse(powerAmount);
            StartCoroutine(ShakeCooldown(impulseSource.ImpulseDefinition.ImpulseDuration));
        }
    }

    IEnumerator ShakeCooldown(float cooldown)
    {
        canShake = false;
        yield return new WaitForSeconds(cooldown);
        canShake = true;
    }
}
