using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class DiaryFlip : MonoBehaviour
{
    public GameObject Text;
    private bool isOnRight;
    private bool isOnLeft;
    private int count = 0;
    private SteamVR_Action_Boolean LeftTurn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("LeftTurn");
    private SteamVR_Action_Boolean RightTurn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RightTurn");
    // Start is called before the first frame update
    void Start()
    {
        isOnRight = false;
        isOnLeft = false;
    }

    private void HandAttachedUpdate(Hand hand)
    {
        if (RightTurn.GetState(hand.handType)) {
            Debug.Log("HI");
            if (!isOnRight)
            {
                Text.GetComponent<Text>().text = "Page " + ++count;
                isOnRight = true;
            }
        } else if (LeftTurn.GetState(hand.handType))
        {
            Debug.Log("HI");
            if (!isOnLeft)
            {
                if (--count < 0)
                    count = 0;
                Text.GetComponent<Text>().text = "Page " + count;
                
                isOnLeft = true;
            }
        }
        else
        {
            isOnRight = false;
            isOnLeft = false;
        }
    }
}
