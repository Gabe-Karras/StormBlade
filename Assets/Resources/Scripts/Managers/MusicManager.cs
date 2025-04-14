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
    private Queue<AudioClip> queue;

    // List of song data. True = song loops, false = song plays once
    private List<bool> loopSong;
    private int songIndex = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.volume = volume;
        queue = new Queue<AudioClip>();
        loopSong = new List<bool>();
    }

    // Keep track of source business
    private void Update() {
        if (!GameSystem.IsPaused()) {
            if (!source.isPlaying)
                StopMusic();
        }
    }

    // Plays a looping song. Will add to queue if another song is currently playing
    public void PlayMusic(AudioClip song) {
        // Set this song to loop
        loopSong.Add(true);

        // Enqueue if something is playing
        if (songPlaying) {
            queue.Enqueue(song);
            return;
        }

        // Set audio source clip to be the song and play it
        source.clip = song;
        source.loop = true;
        source.Play();
        songPlaying = true;

        // Increment song data index
        songIndex ++;
    }

    // Enqeue song but play it only once
    public void PlayIntro(AudioClip song) {
        // Set this song to not loop
        loopSong.Add(false);

        // Enqueue if something is playing
        if (songPlaying) {
            queue.Enqueue(song);
            return;
        }

        // Play song a single time
        source.clip = song;
        source.loop = false;
        source.Play();
        songPlaying = true;

        // Increment song data index
        songIndex ++;
    }

    // Abruptly stops the current song from playing
    public void StopMusic() {
        // Stop the music!!
        source.Stop();
        songPlaying = false;

        // If something is enqueued, play it
        if (queue.Count > 0) {
            if (loopSong[songIndex])
                PlayMusic(queue.Dequeue());
            else
                PlayIntro(queue.Dequeue());
        }
    }

    // Stop current song and empty queue
    public void StopAllMusic() {
        source.Stop();
        songPlaying = false;

        while (queue.Count > 0)
            queue.Dequeue();

        songIndex = 0;
        loopSong = new List<bool>();
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
