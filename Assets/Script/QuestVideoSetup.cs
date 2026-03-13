using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public sealed class QuestVideoSetup : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoFileName = "dacia.mp4";

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        string fullPath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = fullPath;

        Debug.Log($"Video path: {fullPath}");

        StartCoroutine(WaitAndPlay());
    }

    private void Start()
    {
        videoPlayer.Prepare();
    }

    IEnumerator WaitAndPlay()
    {
        float timeout = 10f;
        while (!videoPlayer.isPrepared && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (videoPlayer.isPrepared)
        {
            if (!videoPlayer.isPlaying)
                videoPlayer.Play();
            Debug.Log("[ARVideo] ▶️ Play!");
        }
        else
        {
            Debug.LogError("[ARVideo] ❌ Timeout - video nu s-a pregatit");
        }
    }
}