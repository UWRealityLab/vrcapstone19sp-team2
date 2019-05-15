using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class LightSwitch : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;
    public bool maintainMomemntum = true;
    public float momemtumDampenRate = 5.0f;
    public float initialMappingOffset;
    public AudioClip switchSound;

    // public GameObject lamp;
    public float delta = 0.001f;
    private float previousPosition;

    protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;

    protected int numMappingChangeSamples = 5;
    protected float[] mappingChangeSamples;
    protected float prevMapping = 0.0f;
    protected float mappingChangeRate;
    protected int sampleCount = 0;
    // This is a 0-1 number represents the linear distance mapping. Initially, this is the offset.
    protected float linearMapping = 0.0f;

    protected Interactable interactable;

    protected virtual void Awake()
    {
        mappingChangeSamples = new float[numMappingChangeSamples];
        interactable = GetComponent<Interactable>();

    }
    void Start()
    {
        previousPosition = endPosition.localPosition.y;
        this.transform.localPosition = endPosition.localPosition;
        //Debug.Log("start y: " + this.transform.localPosition.y);
    }

    // Hover, check & grab.
    protected virtual void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        // Grab
        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
        }
    }

    protected virtual void HandAttachedUpdate(Hand hand)
    {
        UpdateLinearMapping(hand.transform);

        // Release
        if (hand.IsGrabEnding(this.gameObject))
        {
            hand.DetachObject(gameObject);
        }
    }
    protected virtual void OnDetachedFromHand(Hand hand)
    {
        CalculateMappingChangeRate();
    }

    // Calculate momentum speed using samples.
    protected void CalculateMappingChangeRate()
    {
        //Compute the mapping change rate
        mappingChangeRate = 0.0f;
        int mappingSamplesCount = Mathf.Min(sampleCount, mappingChangeSamples.Length);
        if (mappingSamplesCount != 0)
        {
            for (int i = 0; i < mappingSamplesCount; ++i)
            {
                mappingChangeRate += mappingChangeSamples[i];
            }
            mappingChangeRate /= mappingSamplesCount;
        }
    }

    protected void UpdateLinearMapping(Transform updateTransform)
    {
        prevMapping = linearMapping;
        linearMapping = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(updateTransform));

        // Record sample speed
        mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = (1.0f / Time.deltaTime) * (linearMapping - prevMapping);
        sampleCount++;

        transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping);
    }

    protected float CalculateLinearMapping(Transform updateTransform)
    {
        Vector3 direction = endPosition.position - startPosition.position;
        float length = direction.magnitude;
        direction.Normalize();

        Vector3 displacement = updateTransform.position - startPosition.position;

        return Vector3.Dot(displacement, direction) / length;
    }

    protected virtual void Update()
    {
        if (maintainMomemntum && mappingChangeRate != 0.0f)
        {
            //Dampen the mapping change rate and apply it to the mapping
            mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0.0f, momemtumDampenRate * Time.deltaTime);
            linearMapping = Mathf.Clamp01(linearMapping + (mappingChangeRate * Time.deltaTime));

            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping);
        }
    }


    private void LateUpdate()
    {
        float currentPosition = this.transform.localPosition.y;
        //Debug.Log(previousPosition + "*****" + currentPosition);
        if (!floatEquals(previousPosition, currentPosition) 
            && (floatEquals(currentPosition, startPosition.localPosition.y) || floatEquals(currentPosition, endPosition.localPosition.y)))
        {
            //Debug.Log("inside switch");
            AudioSource audioSource = this.GetComponent<AudioSource>();
            audioSource.clip = switchSound;
            audioSource.Play();
            previousPosition = currentPosition;
        }
        //if (startPosition.localPosition.y - this.transform.localPosition.y <= delta)
        //{
        //    lamp.GetComponentInChildren<Light>().enabled = true;
        //}

        //else if (this.transform.localPosition.y - endPosition.localPosition.y <= delta)
        //{
        //    lamp.GetComponentInChildren<Light>().enabled = false;
        //}
    }


    private bool floatEquals(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.001f;
    }
}
