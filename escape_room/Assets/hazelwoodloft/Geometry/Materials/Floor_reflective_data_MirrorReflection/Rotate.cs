using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Rotate : MonoBehaviour
{
    public virtual void Update()
    {
        this.transform.Rotate(0, Time.deltaTime * 10, 0);
    }

}