using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChainMouseOrbit_C : MonoBehaviour {

    public Transform target;
    public float distance = 10.0f;
    public Vector3 targetOffset;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float x = 0.0f;
    private float y = 0.0f;

    // Use this for initialization
    void Start () {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (transform.GetComponent("Rigidbody"))
        {
            //transform.GetComponent("Rigidbody").freez = true;
        }
    }

    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0f);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position + targetOffset;

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
            transform.position = Vector3.Slerp(transform.position, position, Time.deltaTime * 2f);
        }
    }

    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }



    void OnGUI()
    {
        if (GUI.Button(new Rect(50f, 30f, 100f, 30f), "Reset"))
        {
            SceneManager.LoadScene(Application.loadedLevel);
        }
    }

}//End Class
