using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManagerScript;
using System.Linq;

public class HintsAndNarrativeScript : MonoBehaviour
{
    public GameObject HeadUI;
    public GameObject WatchUI_TaskList;
    public GameObject WatchUI_NewTask;

    private Text HeadHints;
    private Text WatchHints;
    private Text WatchNewTask;
    private List<TaskTypes> currentTasks;
    private List<string> msgList;

    void Start()
    {
        HeadHints = HeadUI.GetComponentInChildren<Text>();
        WatchHints = WatchUI_TaskList.GetComponentInChildren<Text>();
        WatchNewTask = WatchUI_NewTask.GetComponentInChildren<Text>();
        currentTasks = new List<TaskTypes>();
        msgList = new List<string>();
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
        // update tasks list
        List<string> tmp = new List<string>();
        foreach (TaskTypes t in currentTasks)
        {
            tmp.Add(UIContent.TaskToUI[t]);
        }
        WatchHints.text = string.Join("\n", tmp);

        // update message
        if (msgList.Count >= 2)
            msgList.RemoveAt(0);
        msgList.Add(header + UIContent.TaskToUI[task]);
        WatchNewTask.text = string.Join("\n", msgList);
    }
}
