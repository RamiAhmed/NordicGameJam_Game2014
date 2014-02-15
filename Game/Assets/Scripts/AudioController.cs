using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

	public List<AudioClip> AudioClips = new List<AudioClip>();

	private List<AudioSource> _audioSources = new List<AudioSource>();
	private int _currentClip = 0;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < AudioClips.Count; i++) {
			AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
			_audioSources.Add(audioSource);

			audioSource.clip = AudioClips[i];
			audioSource.volume = 0f;
			audioSource.loop = true;
			audioSource.Play();
		}

		_currentClip = 0;
		_audioSources[_currentClip].volume = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
			ChangeClip(_currentClip+1);
		}
	}

	public void ChangeClip(int newClip) {
		if (_audioSources[newClip] != null && AudioClips[newClip] != null && newClip != _currentClip) {
			_audioSources[_currentClip].volume = 0f;
			_audioSources[newClip].volume = 1f;
			_currentClip = newClip;
		}
	}
}
