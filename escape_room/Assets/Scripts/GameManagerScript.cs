using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIContent;

public class GameManagerScript : MonoBehaviour
{
    public bool debug = false;

    public enum TaskTypes
    {
        RELEASE,
        RADIO,
        DESK,
        FIND_FUSE,
        FIND_MUSIC_BOX,
        FIND_FLARE,
        LIGHT,
        KEY_HINT,
        KEY,
        ICE,
        AMMO,
        CURTAIN
    }

    public enum EventTypes
    {
        WAKE_UP,
        CUTTER_CUT,
    }

    public HashSet<TaskTypes> completedTasks;
    public HashSet<TaskTypes> activeTasks;

    // Start is called before the first frame update
    void Start()
    {
        completedTasks = new HashSet<TaskTypes>();
        activeTasks = new HashSet<TaskTypes>();
    }

    public void TriggerTask(TaskTypes task, int delay=0)
    {
        StartCoroutine(TriggerDelay(task, delay));
    }

    IEnumerator TriggerDelay(TaskTypes task, int delay)
    {
        yield return new WaitForSeconds(delay);
        if (!completedTasks.Contains(task))
        {
            log("Trigger: " + task.ToString());

            activeTasks.Add(task);
        }
    }

    public void CompleteTask(TaskTypes task)
    {
        log("Complete: " + task.ToString());
        activeTasks.Remove(task);
        completedTasks.Add(task);
    }

    public string GetTasks()
    {
        string tasks = "";
        foreach (TaskTypes task in activeTasks)
        {
            tasks += TaskToUI[task] + "\n";
        }
        return tasks.Substring(tasks.Length - 1);
    }

    private void log(string s)
    {
        if (debug)
        {
            Debug.Log(s);
        }
    }
}
