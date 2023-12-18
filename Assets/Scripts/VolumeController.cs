using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{

    public float masterVolume;
    public float musicVolume;
    public float effectsVolume;
    
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup effectsMixerGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        masterMixerGroup.audioMixer.SetFloat("MasterVolume", masterVolume);
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", musicVolume);
        effectsMixerGroup.audioMixer.SetFloat("EffectsVolume", effectsVolume);
    }
}
