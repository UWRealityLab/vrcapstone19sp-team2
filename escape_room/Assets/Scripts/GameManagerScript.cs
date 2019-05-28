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
        AFTER_RADIO_MILITARY,
        AFTER_FLASHLIGHT,
        AFTER_PICK_UP_DIARY,
        AFTER_DIARY_FUSE_PAGE,
        AFTER_DIARY_MUSIC_PAGE,
        AFTER_DIARY_FLARE_GUN,
        AFTER_LIGHT_ON,
        MUSIC_BOX_TOUCHED,
        AFTER_CLIP_BOARD,
        ICE_CUBE_TOUCHED,
        PICKED_UP_KEY,
        MUSIC_BOX_KEY_INSERTED,
        MUSIC_BOX_FINISHED,
        ENTERED_SECRET_ROOM,
        PICKED_UP_GUN,
        EXITED_SECRET_ROOM,
        SAFEBOX_CABINET_OPENED,
        SAFEBOX_OPENED,
        GUN_LOADED,
        CURTAIN_OPENED,
        GLASS_BROKEN,
        FLARE_GUN_FIRED,
        HELI_ARRIVED,
        ESCAPED
    }

    public HashSet<TaskTypes> completedTasks;
    public HashSet<TaskTypes> activeTasks;
    public GameObject UIDisplaySystem;

    private HintsAndNarrativeScript UIDisplay;

    // Start is called before the first frame update
    void Start()
    {
        completedTasks = new HashSet<TaskTypes>();
        activeTasks = new HashSet<TaskTypes>();
        UIDisplay = UIDisplaySystem.GetComponent<HintsAndNarrativeScript>();
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
            UIDisplay.updateTasks(GetTasks(), task);
        }
        
    }

    public void CompleteTask(TaskTypes task)
    {
        log("Complete: " + task.ToString());
        activeTasks.Remove(task);
        completedTasks.Add(task);
        UIDisplay.completeTask(GetTasks(), task);
    }

    public string GetTasks()
    {
        string tasks = "";
        foreach (TaskTypes task in activeTasks)
        {
            tasks += " - " + TaskToUI[task] + "\n";
        }
        if (activeTasks.Count == 0)
        {
            return "";
        }
        return tasks.Substring(0, tasks.Length - 1);
    }

    private void log(string s)
    {
        if (debug)
        {
            Debug.Log(s);
        }
    }
    
    public void TiggerEvent(EventTypes e, int delay) {
        StartCoroutine(TriggerEventDelay(e, delay));
    }

    IEnumerator TriggerEventDelay(EventTypes e, int delay)
    {
        yield return new WaitForSeconds(delay);
        string words = EventToUI[e];
        UIDisplay.updateEventUI(words, words.Split(' ').Length / 2);
    }
}
