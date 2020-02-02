using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    public AudioClip sound;
    private Button button { get { return GetComponent<Button>(); } }
    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    private AudioMixer mixer { get { return Resources.Load<AudioMixer>("AudioMixer");  } }

    void Awake()
    {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = mixer.FindMatchingGroups("UI")[0];
        
        button.onClick.AddListener(() => PlaySound());
    }

    public void StopSound()
    {
        if (source == null)
        {
            return;
        }
        source.Stop();
    }

    void PlaySound()
    {
        source.PlayOneShot(sound);
    }

}
