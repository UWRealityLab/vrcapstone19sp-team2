using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecreteDoorSwitch : MonoBehaviour
{
    public Animator ScreteDoor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        StartCoroutine(OpenWait());
    }

    IEnumerator OpenWait()
    {
        // Disable door circular 
        // To disable hovering
        ScreteDoor.SetTrigger("move");
        yield return new WaitForSeconds(5);
        // To enable hovering:
        ScreteDoor.SetTrigger("move");
    }
}
