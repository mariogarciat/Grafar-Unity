using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using UnityEngine.Networking;
using ZXing.QrCode;
using Vuforia;

public class QRScript : MonoBehaviour
{
    public RawImage panel;
    public VuforiaMonoBehaviour vuforiaBehavior;
    private WebCamTexture camTexture;


    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status status;

    public SpriteRenderer marcadorUno;
    [SerializeField]
    private Sprite imagenACambiar;

    private bool requestFinished;
    public DefaultTrackableEventHandler defaultTrackableEventHandler;

    void Start()
    {
        if (camTexture == null)
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
                CancelInvoke("decodeQR");
                panel.gameObject.SetActive(false);
                vuforiaBehavior.enabled = true;
                ObtenerImagen(result.Text);
            }
            else
            {
                Debug.Log("SIN RESULTADOS");
            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }


    public void ObtenerImagen(string jsonData)
    {
        Debug.LogError("ObtenerImagen");
        //cositas request
        string url = "https://grafar.herokuapp.com/api/data";
        StartCoroutine(GetData(url, jsonData));
    }

    IEnumerator GetData(string uri, string jsonData)
    {
        Debug.LogError("GetData");
        //string function = "x^3+11";
        //int a = -20;
        //int b = 20;
        //string jsonData = "{\"function\": \"" + function + "\",\"a\": " + a + ",\"b\": " + b + "}";
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest webRequest = UnityWebRequest.Post(uri, "POST");
        webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        requestFinished = true;
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);
            requestFinished = false;
        }
        else
        {
            Debug.Log("Response: " + webRequest.downloadHandler.text);
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(webRequest.downloadHandler.text);
            if (response.success)
            {
                imagenACambiar = setImage(response.message);
                marcadorUno.sprite = imagenACambiar;
            }
        }
    }

    private Sprite setImage(string base64Image)
    {
        byte[] array = System.Convert.FromBase64String(base64Image);

        Texture2D text = new Texture2D(1, 1);
        text.LoadImage(array);
        return Sprite.Create(text, new Rect(0, 0, text.width, text.height), Vector2.zero);
    }

    public class ApiResponse
    {
        public string message;
        public bool success;
    }
}
