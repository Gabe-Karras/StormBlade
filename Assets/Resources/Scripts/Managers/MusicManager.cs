using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // 1 = max
    [SerializeField]
    private float volume;

    // How long to completely fade out a song (seconds)
    [SerializeField]
    private float fadeSpeed;

    private AudioSource source;
    private bool songPlaying = false;

    // Queue to hold song order!
    Queue<AudioClip> queue;
    
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = volume;
        queue = new Queue<AudioClip>();
    }

    // Plays a looping song. Will add to queue if another song is currently playing
    public void PlayMusic(AudioClip song) {
        // Enqueue if something is playing
        if (songPlaying) {
            queue.Enqueue(song);
            return;
        }

        // Set audio source clip to be the song and play it
        source.clip = song;
        source.Play();
        songPlaying = true;
    }

    // Abruptly stops the current song from playing
    public void StopMusic() {
        // Stop the music!!
        source.Stop();
        songPlaying = false;

        // If something is enqueued, play it
        if (queue.Count > 0)
            PlayMusic(queue.Dequeue());
    }

    // Stop current song and empty queue
    public void StopAllMusic() {
        source.Stop();
        songPlaying = false;

        while (queue.Count > 0)
            queue.Dequeue();
    }

    // Fades out the current song
    public IEnumerator FadeOut() {
        // Caluclate vars to fade sound over time
        float stepTime = 0.05f; // 20th of a second
        float stepAmount = volume / (fadeSpeed / stepTime);

        while (source.volume > 0) {
            source.volume -= stepAmount;
            yield return new WaitForSeconds(stepTime);
        }

        // Once song is faded out, officially end it and restore volume
        StopMusic();
        source.volume = volume;
    }

    // Pause the current song
    public void PauseMusic() {
        source.Pause();
    }

    // Resume current song
    public void ResumeMusic() {
        source.Play();
    }
}
