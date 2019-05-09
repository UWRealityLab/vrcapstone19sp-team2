using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWindowCollider : MonoBehaviour
{

    public GameObject window;
    public GameObject actualWindow;
    // Start is called before the first frame update
    void Start()
    {
        window.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {;
        Debug.Log("col: " + col.gameObject.name);
        if (col.gameObject.name == "Bullet(Clone)")
        {
            actualWindow.SetActive(false);
            window.GetComponent<MeshRenderer>().enabled = true;
            window.GetComponent<BreakableWindow>().breakWindow();

//            StartCoroutine(Break());
        }
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(1);
        window.GetComponent<BreakableWindow>().breakWindow();
    }
}
