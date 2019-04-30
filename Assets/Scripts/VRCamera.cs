using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCamera : MonoBehaviour
{
    public VuforiaMonoBehaviour vuforiaBehavior;

    // Start is called before the first frame update
    void Start()
    {
        vuforiaBehavior.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activarCamara()
    {
        vuforiaBehavior.enabled = true;
    }
}
