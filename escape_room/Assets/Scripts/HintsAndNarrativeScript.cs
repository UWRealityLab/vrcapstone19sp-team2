using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManagerScript;

public class HintsAndNarrativeScript : MonoBehaviour
{
    public GameObject HeadUI;
    public GameObject WatchUI_TaskList;
    public GameObject WatchUI_NewTask;

    private Text HeadHints;
    private Text WatchHints;
    private Text WatchNewTask;
    private List<TaskTypes> currentTasks;
    private List<TaskTypes> msgList;

    void Start()
    {
        HeadHints = HeadUI.GetComponentInChildren<Text>();
        WatchHints = WatchUI_TaskList.GetComponentInChildren<Text>();
        WatchNewTask = WatchUI_NewTask.GetComponentInChildren<Text>();
        currentTasks = new List<TaskTypes>();
        msgList = new List<TaskTypes>();
    }

    public void addTask(TaskTypes task)
    {
        currentTasks.Add(task);
        refreshWatchUI("New: ", task);
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

    public void completeTask(TaskTypes completedTask)
    {
        currentTasks.Remove(completedTask);
        refreshWatchUI("Completed: ", completedTask);
    }

    private void refreshWatchUI(string header, TaskTypes task)
    {
        string hint = string.Join("\n", currentTasks);
        WatchHints.text = hint;
        WatchNewTask.text = header + UIContent.TaskToUI[task];
    }


}
