using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class Rocket : MonoBehaviour
{
    public AudioSource[] flameThrowerAudioSources;
    public ParticleSystem[] fireParticleSystems;
    public ParticleSystem[] smokeParticleSystems;
    public Text coundownText;
    private int countdown = 30;
    private float lastTime = -1;

    public void launch()
    {
        lastTime = Time.time;
        Invoke("fireEngines", 21);
        Invoke("startAnimating", 31);
    }
    public void fireEngines()
    { 
        for (int index = 0; index < flameThrowerAudioSources.Length; index++)
        {
            flameThrowerAudioSources[index].Play();
            fireParticleSystems[index].Play();
            smokeParticleSystems[index].Play();
        }
    }

    public void startAnimating()
    {
        GetComponent<Animator>().SetBool("launch", true);
    }

    public void launched()
    {
        SteamVR_LoadLevel.Begin("moonSceneMain", false, 2f);
    }

    void Update()
    {
        if (Time.time - lastTime >= 1 && countdown >= 0 && lastTime != -1) 
        {
            string number = "" + countdown;
            coundownText.text = "00:" + (number.Length == 1 ? "0" : "") + number;
            lastTime = Time.time;
            countdown--;
        }

    }
}
