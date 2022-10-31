using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public float MaxDown = -15;
    private void Start()
    {
        mixer.SetFloat("BgVol", 0);
    }

    public void SetDown()
    {
        //mixer.SetFloat("BgVol",-10);
        // mixer.SetFloat("BgVol",Mathf.Lerp(0, -20, Time.time));
        StartCoroutine(AudioFadeOut.FadeOut(mixer, 0.1f, MaxDown));
    }
    public void SetUp()
    {
        //mixer.SetFloat("BgVol", 0);
        // mixer.SetFloat("BgVol", Mathf.Lerp(-20, 0, Time.time));
        StartCoroutine(AudioFadeIn.FadeOut(mixer, 0.1f, MaxDown));
    }

    public static class AudioFadeOut
    {

        public static IEnumerator FadeOut(AudioMixer mixer, float FadeTime, float maxDown)
        {
            float i = 0;
           
            while (i > maxDown)
            {
                mixer.SetFloat("BgVol", i - (i - 0.1f) * Time.deltaTime / FadeTime);
                i -= 0.1f;
                yield return null;
            }
            mixer.SetFloat("BgVol", maxDown);
            //  i -= 0.1f;
        }

    }

    public static class AudioFadeIn
    {

        public static IEnumerator FadeOut(AudioMixer mixer, float FadeTime, float maxDown)
        {
            float i = maxDown;

            while (i <0)
            {
                mixer.SetFloat("BgVol", i + (i + 0.1f) * Time.deltaTime / FadeTime);
                i += 0.1f;
                yield return null;
            }

            mixer.SetFloat("BgVol", 0);
        }
        

    }
}
