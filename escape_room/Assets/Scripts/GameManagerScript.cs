﻿using System.Collections;
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
        NULL,
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
        SECRETE_DOOR_OPEN, // When door open finishes
        ENTERED_SECRET_ROOM,
        PICKED_UP_GUN,
        EXITED_SECRET_ROOM,
        SAFEBOX_CABINET_OPEN,
        SAFEBOX_OPEN,
        GUN_LOADED,
        CURTAIN_OPEN,
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

    public void TriggerTask(TaskTypes task, EventTypes afterUI=EventTypes.NULL, int extraDelay = 0)
    {
        StartCoroutine(TriggerDelay(task, afterUI == EventTypes.NULL ? 0 + extraDelay : getUILength(afterUI) + extraDelay));
    }

    IEnumerator TriggerDelay(TaskTypes task, int delay)
    {
        yield return new WaitForSeconds(delay);
        if (!activeTasks.Contains(task) && !completedTasks.Contains(task))
        {
            log("Trigger: " + task.ToString());

            activeTasks.Add(task);
            UIDisplay.updateTasks(GetTasks(), task);
        }
        
    }

    public void CompleteTask(TaskTypes task)
    {
        if (!completedTasks.Contains(task))
        {
            log("Complete: " + task.ToString());
            activeTasks.Remove(task);
            completedTasks.Add(task);
            UIDisplay.completeTask(GetTasks(), task);
        }
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
    
    public void TriggerEvent(EventTypes e, int delay = 0) {
        StartCoroutine(TriggerEventDelay(e, delay));
    }

    IEnumerator TriggerEventDelay(EventTypes e, int delay=0)
    {
        yield return new WaitForSeconds(delay);
        string words = EventToUI[e];
        UIDisplay.updateEventUI(words, getUILength(e));
    }

    public static int getUILength(EventTypes e) {
        return Mathf.Max(EventToUI[e].Split(' ').Length / 2, UIContent.UI_MIN_DELAY_SECONDS);
    }
}
