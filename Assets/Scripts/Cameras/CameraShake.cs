using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    private void OnEnable()
    {
        BallController.OnHealthChangeEvent += Shake;
    }

    private void OnDisable()
    {
        BallController.OnHealthChangeEvent -= Shake;
    }

    public void Shake(float percentage)
    {
        impulseSource.GenerateImpulse();
    }
}
