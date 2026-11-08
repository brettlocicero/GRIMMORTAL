using UnityEngine;
using Unity.Cinemachine;

public class CinemachineShakeManager : MonoBehaviour
{
    public static CinemachineShakeManager Instance;

    [SerializeField] CinemachineBasicMultiChannelPerlin cbmcp;
    [SerializeField] float smoothSpeed = 15f;

    float shakeTimer;
    float shakeTimerTotal;
    float startingIntensity;
    float targetFrequency;
    float priority;

    void Awake()
    {
        Instance = this;
    }

    public void ShakeCamera(float intensity, float time, float shakeFreq, float priorityRequest)
    {
        if (priorityRequest >= priority)
        {
            startingIntensity = intensity;
            targetFrequency = shakeFreq;
            shakeTimerTotal = time;
            shakeTimer = time;
            priority = priorityRequest;
        }
    }

    void Update()
    {
        shakeTimer = Mathf.Max(0f, shakeTimer - Time.deltaTime);

        float decay = shakeTimerTotal > 0f ? shakeTimer / shakeTimerTotal : 0f;
        float targetAmplitude = startingIntensity * decay;
        float t = Time.deltaTime * smoothSpeed;

        cbmcp.AmplitudeGain = Mathf.Lerp(cbmcp.AmplitudeGain, targetAmplitude, t);
        cbmcp.FrequencyGain = Mathf.Lerp(cbmcp.FrequencyGain, targetFrequency, t);

        if (shakeTimer <= 0f) priority = 0f;
    }
}