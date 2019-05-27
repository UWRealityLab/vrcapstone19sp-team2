using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public enum clockType
{
classic, modern
}

public class Watch : MonoBehaviour {

    //se this for initialization

    //this allows simply to change the clock/watch type
    public clockType type;
    //these are the canvases with the watch parameters
    public Canvas classicWatch, modernWatch;
    public GameObject sphere, ortoedre;

    // angle parameters
    float offsetAngle = 0f; // Think of this like a time zone
    

    //transforms used for the classic watch
    Transform hourT, minT, secondT;

    //know time publically
    public float hourR ;
    public float minuteR;
    public float secondR;


    //texts used for modern watch
    public Text txtHour, txtMin, txtSec;

    void Start () {

        //show or hide the objects that correspond to the style
		if(type==clockType.classic)
        {
            sphere.SetActive(true);
            ortoedre.SetActive(false);
            classicWatch.enabled = true;
            modernWatch.enabled = false;

            hourT = classicWatch.transform.GetChild(0);
            minT = classicWatch.transform.GetChild(1);
            secondT = classicWatch.transform.GetChild(2);


        }
        else
        {
            sphere.SetActive(false);
            ortoedre.SetActive(true);
            classicWatch.enabled = false;
            modernWatch.enabled = true;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {      
        //get time data
        DateTime currentTime = System.DateTime.Now;
        hourR = currentTime.Hour;
        minuteR = currentTime.Minute;
        secondR = currentTime.Second;

        if (type == clockType.classic)
        {
            //set rotations of the gameobjects in case that is a classic watch
            secondT.localRotation = Quaternion.Euler(0, 0, secondR * 6 + 180);
            minT.localRotation = Quaternion.Euler(0, 0, minuteR * 6 + 180);
            hourT.localRotation = Quaternion.Euler(0, 0, hourR * 30 + 180);
        }
        else
        {
            //set the text values in cas it is a modern watch
            //set the text 0+value  or value
            if(secondR<10)
            {
                txtSec.text = "0" + secondR;
            }
            else
            {
                txtSec.text =""+ secondR;
            }

            if (minuteR < 10)
            {
                txtMin.text = "0" + minuteR;
            }
            else
            {
                txtMin.text =""+ minuteR;
            }

            if (hourR < 10)
            {
                txtHour.text = "0" + hourR;
            }
            else
            {
                txtHour.text = "" + hourR;
            }
        }


    }
}
