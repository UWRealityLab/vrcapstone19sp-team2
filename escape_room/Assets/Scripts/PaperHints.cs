using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperHints : MonoBehaviour
{
    public GameObject paper;

    public void EnablePaperHintTrigger()
    {
        paper.GetComponentInChildren<Text>().enabled = true;
    }

    public void DisablePaperHintTrigger()
    {
        paper.GetComponentInChildren<Text>().enabled = false ;
    }
}
