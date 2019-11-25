using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	public Sound[] sounds;

	//Last time a sound started kept stored for play repeatedly
	private float laststart = 0f;

	void Awake()
	{
		if (!instance)
		{
			instance = this;

			foreach (Sound s in sounds)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;
				s.source.volume = s.volume;
				s.source.pitch = s.pitch;
				s.source.loop = s.loop;
			}
		}

		else
		{
			Destroy(gameObject);
		}
	}
	
	public void PlayRepeatedly(string name, float pitch)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.pitch = pitch;
		if(Time.time > laststart+0.1f)
		{
			s.source.Play();
			laststart = Time.time;
		}
	}

	public void PlayPitched(string name, float pitch, bool overlap)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (overlap)
		{
			s.source.pitch = pitch;
			s.source.PlayOneShot(s.source.clip);
		}

		else
		{
			if (!s.source.isPlaying)
			{
				s.source.PlayOneShot(s.source.clip);
			}
		}
	}

	public void PlayPitched(string name, float pitch)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.pitch = pitch;
		if (!s.source.isPlaying)
		{
			s.source.PlayOneShot(s.source.clip);
		}
	}

	public void Play(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.PlayOneShot(s.source.clip);
	}
}