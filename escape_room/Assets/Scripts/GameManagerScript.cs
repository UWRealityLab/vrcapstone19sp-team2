using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIContent;

public class GameManagerScript : MonoBehaviour
{
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

    public void TriggerTask(TaskTypes task)
    {
        if (!completedTasks.Contains(task))
        {
            activeTasks.Add(task);
        }
    }

    public void CompleteTask(TaskTypes task)
    {
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
}
