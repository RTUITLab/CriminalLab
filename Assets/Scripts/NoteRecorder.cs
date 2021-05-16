using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NoteRecorder : MonoBehaviour
{
    public string AudioNotesFolder = "Audio Notes";
    public static bool IsRecording = false;
    private bool isRecording = false;
    public static bool IsPlaying = false;

    [Space(10)]
    public Sprite ImageStart;
    public Sprite ImageStop;
    public Sprite ImagePlayStart;
    public Sprite ImagePlayStop;
    public Sprite ImagePlayLoading;
    public Sprite ImagePlayError;
    public Sprite ImagePlayEmpty;

    [Space(10)]
    public string audioClipPath;

    private AudioClip audioClip;
    private AudioSource audioSource;

    private string deviceName = "";
    private float startRecordingTime;
    private Text Timer;
    private Coroutine timerCoroutine;
    private GameObject DeleteNoteButton;

    void Awake()
    {
        DeleteNoteButton = transform.parent.GetChild(0).GetChild(0).gameObject;

        audioSource = transform.GetChild(1).GetComponent<AudioSource>();

        Timer = transform.GetChild(2).GetComponent<Text>();
        if (string.IsNullOrEmpty(audioClipPath))
        {
            Timer.text = "Записи нет";
            transform.GetChild(1).GetComponent<Image>().sprite = ImagePlayEmpty;
        }
        else
        {
            Timer.text = "Запись есть";
        }
    }

    public void DeleteNote()
    {
        Debug.Log("Вы удалили заметку!");
    }

    public void ToggleRecording()
    {
        if (IsRecording != isRecording)
        {
            return;
        }

        if (!IsRecording)
        {
            ChangeRecordingState(true);
            StartRecording();
        }
        else
        {
            ChangeRecordingState(false);
            StopRecording();
        }
    }

    public void TogglePlayAudioClip()
    {
        if (IsPlaying != audioSource.isPlaying)
        {
            return;
        }

        if (audioSource.isPlaying)
        {
            ChangeAudioNotePlayState(false);
            return;
        }
        if (!audioSource.isPlaying && audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Loaded)
        {
            ChangeAudioNotePlayState(true);
            return;
        }
    }

    private void StopPlaying()
    {
        if (transform.GetChild(1).GetComponent<Image>().sprite != ImagePlayStart)
        {
            ChangeAudioNotePlayState(false);
        }
    }

    public void SetAudioClipPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        Timer = transform.GetChild(2).GetComponent<Text>();
        audioClipPath = path;
        Timer.text = "Запись есть";
        transform.GetChild(1).GetComponent<Image>().sprite = ImagePlayStart;
        GetAudioClip();
    }

    private void ChangeRecordingState(bool newState)
    {
        IsRecording = isRecording = newState;
        DeleteNoteButton.SetActive(!IsRecording);
        transform.GetChild(0).GetComponent<Image>().sprite = IsRecording ? ImageStop : ImageStart;
    }

    private void ChangeAudioNotePlayState(bool newState)
    {
        IsPlaying = newState;

        if (IsPlaying)
        {
            audioSource.Play();
            Invoke(nameof(StopPlaying), audioSource.clip.length);
        }
        else
        {
            audioSource.Stop();
            CancelInvoke(nameof(StopPlaying));
        }

        DeleteNoteButton.SetActive(!IsPlaying);
        transform.GetChild(1).GetComponent<Image>().sprite = IsPlaying ? ImagePlayStop : ImagePlayStart;
    }

    private void StartRecording()
    {
        // TODO: if re-recording - delete old file
        // If audio path is not empty - delete old file

        if (Microphone.IsRecording(deviceName))
        {
            return;
        }

        int minFreq, maxFreq = 0;
        int avFreq = 44100;
        audioClip = null;
        deviceName = Microphone.devices[0];
        Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);

        Debug.Log(deviceName);

        avFreq = maxFreq < avFreq ? maxFreq : avFreq;

        audioClip = Microphone.Start(deviceName, false, 300, avFreq);
        startRecordingTime = Time.time;

        timerCoroutine = StartCoroutine(StartTimer());
        ChangeRecordingState(true);
    }

    private void StopRecording()
    {
        if (!Microphone.IsRecording(deviceName))
        {
            return;
        }

        Microphone.End(deviceName);

        if (audioClip == null)
        {
            return;
        }

        StopCoroutine(timerCoroutine);
        Timer.text = "Запись есть";
        ChangeRecordingState(false);

        try
        {
            AudioClip recordingNew = AudioClip.Create(audioClip.name,
                (int)((Time.time - startRecordingTime) * audioClip.frequency),
                audioClip.channels,
                audioClip.frequency,
                false);
            float[] data = new float[(int)((Time.time - startRecordingTime) * audioClip.frequency)];
            audioClip.GetData(data, 0);
            recordingNew.SetData(data, 0);
            this.audioClip = recordingNew;

            string recordName = $"Запись от {DateTime.Now:dd-MM-yyyy HH-mm-ss}";
            string pathToSave = Path.Combine("Resources", AudioNotesFolder, recordName);
            Debug.Log(pathToSave);
            SavWav.Save(pathToSave, audioClip);
            AssetDatabase.Refresh();
            SetAudioClipPath(Path.Combine(AudioNotesFolder, recordName));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.StackTrace + "\n\n" + ex.Message);
            Timer.text = "Произошла ошибка";
        }
    }

    private IEnumerator StartTimer()
    {
        TimeSpan time = TimeSpan.FromMinutes(5);
        while (true)
        {
            Timer.text = "Осталось времени:\n" + time.ToString(@"m\:ss");
            time -= TimeSpan.FromSeconds(1);
            yield return new WaitForSeconds(1);
        }
    }

    private void GetAudioClip()
    {
        transform.GetChild(1).GetComponent<Image>().sprite = ImagePlayLoading;
        try
        {
            audioSource.clip = Resources.Load<AudioClip>(audioClipPath);
            transform.GetChild(1).GetComponent<Image>().sprite = ImagePlayStart;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.StackTrace + "\n\n" + ex.Message);
            transform.GetChild(1).GetComponent<Image>().sprite = ImagePlayError;
        }
    }
}
