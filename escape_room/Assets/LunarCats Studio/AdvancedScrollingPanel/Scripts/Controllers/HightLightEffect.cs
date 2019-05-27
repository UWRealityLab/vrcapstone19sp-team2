using UnityEngine;
using System.Collections;

public class HightLightEffect : MonoBehaviour {

	/// 
	/// Public variables declaration
	///
	public Color32 colorFrom = new Color32(0, 0, 0, 255);		// Original color
	public Color32 colorTo = new Color32(200, 200, 60, 255);	// Target color when Highlighted
	public float frequence = 0.7f;								// Frequence of the cycle
	public float pauseTime = 0f;								// Duration of the pause between to cycles

	/// 
	/// Private variables declaration
	/// 
	private float animationTimer = 0f;							// Main timer for animation managment 
	private float pauseTimer = 0f;								// Timer for pause managment
	private Material mat;										// Reference to the Material object of the Renderer

	// Use this for initialization
	void Start () {
		mat = this.GetComponent<Renderer> ().material;
		mat.SetColor ("_EmissionColor", colorFrom);
		mat.SetFloat ("_EmissionScaleUI", 1f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (animationTimer < frequence) 
		{
			animationTimer += Time.deltaTime;
			mat.SetColor ("_EmissionColor", Color32.Lerp(colorFrom, colorTo, animationTimer / frequence));
		}
		else
		{
			if(pauseTimer < pauseTime) {
				pauseTimer += Time.deltaTime;
			}
			else if(animationTimer < 2*frequence)
			{
				animationTimer += Time.deltaTime;
				mat.SetColor ("_EmissionColor", Color32.Lerp(colorTo, colorFrom, (animationTimer - frequence) / frequence));
			}
			else if(pauseTimer < 2*pauseTime)
			{
				pauseTimer += Time.deltaTime;
			}
			else
			{
				pauseTimer = 0f;
				animationTimer = 0f;
			}
		}
	}

	public void UpdateMaterial()
	{
		mat = this.GetComponent<Renderer> ().material;
	}

	public void SetTimeReference(float time_p)
	{
		animationTimer = time_p;
	}

	public float GetTimeReference()
	{
		return animationTimer;
	}

	public void ResetColor()
	{
		mat.SetColor ("_EmissionColor", colorFrom);
		mat.SetFloat ("_EmissionScaleUI", 0.1f);
	}
}
