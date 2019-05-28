using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsAndNarrativeScript : MonoBehaviour
{
    public GameObject HeadUI;
    public GameObject WatchUI;

    private Text HeadHints;
    private Text WatchHints;

    void Start()
    {
        HeadHints = HeadUI.GetComponentInChildren<Text>();
        WatchHints = WatchUI.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateTasksUI(string hint)
    {
        WatchHints.text = hint;
    }

    public void updateEventUI(string hint)
    {
        StartCoroutine(hint, 3);
    }

    IEnumerator HeadHintsWait(string str, int seconds)
    {
        HeadHints.text = str;
        yield return new WaitForSeconds(seconds);
        HeadHints.text = "";
    }


}
