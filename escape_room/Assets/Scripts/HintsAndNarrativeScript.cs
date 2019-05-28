using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManagerScript;
using System.Linq;
using Valve.VR;

public class HintsAndNarrativeScript : MonoBehaviour
{
    public GameObject HeadUI;
    public GameObject WatchUiTaskList;
    public GameObject WatchUiNewTask;
    // Haptics
    public SteamVR_Action_Vibration hapticAction;

    private Text HeadHints;
    private Text WatchHints;
    private Text WatchNewTask;
    private List<string> msgList;

    void Start()
    {
        HeadHints = HeadUI.GetComponentInChildren<Text>();
        WatchHints = WatchUiTaskList.GetComponentInChildren<Text>();
        WatchNewTask = WatchUiNewTask.GetComponentInChildren<Text>();
        msgList = new List<string>();
    }

    public void updateTasks(string tasksString, TaskTypes task)
    {
        refreshWatchUI(tasksString, "New: ", task);
    }

    public void updateEventUI(string hint, int delay)
    {
        StartCoroutine(hint, delay);
    }

    IEnumerator HeadHintsWait(string str, int seconds)
    {
        HeadHints.text = str;
        yield return new WaitForSeconds(seconds);
        HeadHints.text = "";
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

        // haptic
        hapticAction.Execute(1, 5f, 100, 1, SteamVR_Input_Sources.RightHand);
    }
}
