using UnityEngine;
using System;


public class MouseOrbitCycle:MonoBehaviour{
    public GameObject[] gameObjectArray;
    public float distance = 3.0f;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public int yMinLimit = -360;
    public int yMaxLimit = 360;
    public int zoomRate = 25;
    
    Transform currentTarget;
    float x = 0.0f;
    float y = 0.0f;
    int currentCounter =0;
    
    public void Start() {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        currentTarget = gameObjectArray[0].transform;
    }
    
    public void Update() {
    	if (gameObjectArray != null) {
    		if(Input.GetMouseButtonUp(0)){
    		if(gameObjectArray.Length > currentCounter+1)
    		currentCounter++;
    		else
    		currentCounter =0;
    		currentTarget = gameObjectArray[currentCounter].transform;
    		
    		}
    		
    		
    		x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            distance += -(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);      		
     		y = ClampAngle(y, (float)yMinLimit, (float)yMaxLimit);
            Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + currentTarget.position;      
            transform.rotation = rotation;
            transform.position = position;  
        }
    }
    
    public static float ClampAngle(float angle,float min,float max) {
    	if (angle < -360)
    		angle += 360.0f;
    	if (angle > 360)
    		angle -= 360.0f;
    	return Mathf.Clamp (angle, min, max);
    }
}