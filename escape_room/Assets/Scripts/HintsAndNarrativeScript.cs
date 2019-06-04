using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManagerScript;
using System.Linq;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HintsAndNarrativeScript : MonoBehaviour
{
    public GameObject HeadUI;
    public GameObject WatchUiTaskList;
    public GameObject WatchUiNewTask;
    public GameObject Watch;
    public AudioClip WatchNoticeSound;
    public AudioSource HeadSetAudioSource;
    //public GameObject CanvasBackground;
    // Haptics
    public SteamVR_Action_Vibration hapticAction;
    public Hand WatchHand;

    private Text HeadHints;
    private Text WatchHints;
    private Text WatchNewTask;
    private List<string> msgList;
    private AudioSource WatchAudioSource;

    private static Color RED = new Color(173, 0, 38, 255);
    private static Color YELLOW = new Color(203, 246, 53, 255);

    void Start()
    {
        HeadHints = HeadUI.GetComponentInChildren<Text>();
        WatchHints = WatchUiTaskList.GetComponentInChildren<Text>();
        WatchNewTask = WatchUiNewTask.GetComponentInChildren<Text>();
        msgList = new List<string>();
        WatchAudioSource = Watch.GetComponent<AudioSource>();
    }

    public void updateTasks(string tasksString, TaskTypes task)
    {
        refreshWatchUI(tasksString, "New: ", task);
    }

    public void updateEventUI(string hint, int delay, bool red)
    {
        if (red)
        {
            HeadHints.color = RED;
        } else
        {
            HeadHints.color = YELLOW;
        }
        StartCoroutine(HeadHintsWait(hint, delay));
    }

    public void updateEventNarrative(AudioClip audio)
    {
        HeadSetAudioSource.clip = audio;
        HeadSetAudioSource.Play();
    }

    IEnumerator HeadHintsWait(string str, int seconds)
    {
        //CanvasBackground.SetActive(true);
        HeadHints.text = str;
        yield return new WaitForSeconds(seconds);
        HeadHints.text = "";
        //CanvasBackground.SetActive(false);
    }

    public void completeTask(string taskString, TaskTypes completedTask)
    {
        refreshWatchUI(taskString, "Completed: ", completedTask);
    }

    private void refreshWatchUI(string tasksString, string header, TaskTypes task)
    {
        // update tasks list
        WatchHints.text = tasksString;

        // update message
        if (msgList.Count >= 2)
            msgList.RemoveAt(0);
        msgList.Add(header + UIContent.TaskToUI[task]);
        WatchNewTask.text = string.Join("\n", msgList);

        // haptic and sound
        if (WatchAudioSource.clip == null)
            WatchAudioSource.clip = WatchNoticeSound;
        WatchAudioSource.Play();
        WatchHand.TriggerHapticPulse(500);
        StartCoroutine(PulseCoroutine());
    }

    private IEnumerator PulseCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        WatchHand.TriggerHapticPulse(500);
    }
}
