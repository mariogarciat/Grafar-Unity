using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class CambioImagen : MonoBehaviour
{
    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status status;

    public SpriteRenderer marcadorUno;
    [SerializeField]
    private Sprite imagenACambiar;

    private bool requestFinished;
    public DefaultTrackableEventHandler defaultTrackableEventHandler;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        status = defaultTrackableEventHandler.getStatus();
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        Debug.Log("STATUS: " + status);
        if (status == TrackableBehaviour.Status.TRACKED && !requestFinished)
        {
            Debug.Log("ENTRE!!!!");
            //ObtenerImagen();         
        }
    }

    public void ObtenerImagen(string jsonData)
    {
        Debug.LogError("ObtenerImagen");
        //cositas request
        string url = "https://grafar.herokuapp.com/api/data";
        GetData(url, jsonData);
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
        byte [] array = System.Convert.FromBase64String(base64Image);

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
