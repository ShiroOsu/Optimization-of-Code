using UnityEngine;
using Cinemachine;

public class Camshaker : MonoBehaviour
{
    private SpriteRenderer sr;
    private float fadeSpeed = 2f;
    public Color redColor, normalColor;



    //Cinemachine & camera stuff
    private CinemachineBasicMultiChannelPerlin vCamNoise = null;
    public CinemachineVirtualCamera vCam;
    public Camera cam;
    public float shakeDuration = 0.2f;
    public float shakeAmplitude = 1f;
    public float shakeFrequency = 2f;
    private float shakeTimer = 0f;

    private void Awake()
    {
        //if (vCam != null)
        //{
        //    vCamNoise = vCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        //}

        //if()

        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Shaketest
        if (Input.GetKey(KeyCode.Z))
        {
            Shake();
        }

        if (vCam != null || vCamNoise != null)
        {
            if (shakeTimer > 0)
            {
                vCamNoise.m_AmplitudeGain = shakeAmplitude;
                vCamNoise.m_FrequencyGain = shakeFrequency;

                sr.color = Color.Lerp(sr.color, Color.red, Time.deltaTime * fadeSpeed);
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                vCamNoise.m_AmplitudeGain = 0f;
                sr.color = Color.Lerp(sr.color, Color.white, Time.deltaTime * fadeSpeed);
                shakeTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Shake();
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;


        

        //sr.color = Color.Lerp(sr.color, Color.red, Time.deltaTime * fadeSpeed);
        //

    }

}
