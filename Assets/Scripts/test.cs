using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class test : MonoBehaviour
{
    public RawImage panel;
    private WebCamTexture camTexture;
    public AspectRatioFitter fit;
    
    void Start()
    {
        if(camTexture == null)
        {
            camTexture = new WebCamTexture();
            camTexture.requestedHeight = Screen.height;
            camTexture.requestedWidth = Screen.width;

            panel.texture = camTexture;
            if (camTexture != null)
            {
                camTexture.Play();
                InvokeRepeating("decodeQR", 2.0f, 0.5f);
            }
        }
        
    }
    
    void Update()
    {
        int orient = -camTexture.videoRotationAngle;
        panel.transform.localEulerAngles = new Vector3(0, 0, orient);

    }

    void decodeQR()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(),
              camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR: " + result.Text);
            }
            else
            {
                Debug.Log("SIN RESULTADOS");
            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }

    public void myfn()
    {
        Debug.LogError("hola desde test");
    }
}
