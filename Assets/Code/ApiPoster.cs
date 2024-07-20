using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Root
{
    public int statusCode;
    public string imageUrl;
    public string originalText;
}

public class ApiPoster : MonoBehaviour
{
    public GameObject planeObject;
    public InputField inputField;
    public TextMeshPro originalTextUI;
    public Button sendButton;
    public Text responseText;
    public string apiUrl = "https://qa4i109oz8.execute-api.us-east-1.amazonaws.com/dev/arposter";

    // Tambahkan objek sesuai permintaan
    public InputField gameObjekTextField;
    public Text gameObjekText;
    public Button gameObjekButton;

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClicked);
        gameObjekButton.onClick.AddListener(OnGameObjekButtonClicked);
    }

    public void OnSendButtonClicked()
    {
        string userText = inputField.text;

        if (!string.IsNullOrEmpty(userText))
        {
            StartCoroutine(SendTextRequest(userText));

            // Menonaktifkan textfield gameobjek
            gameObjekTextField.gameObject.SetActive(false);

            // Mengubah teks pada text gameobjek
            gameObjekText.text = "Buy Poster";

            // Mengaktifkan button gameobjek2
            gameObjekButton.gameObject.SetActive(true);

            // Menonaktifkan button sendButton
            sendButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Teks tidak boleh kosong.");
        }
    }

    // Metode yang akan dipanggil ketika button gameobjek2 diklik
    public void OnGameObjekButtonClicked()
    {
        // Tambahkan logika atau tindakan yang ingin dilakukan ketika button gameobjek2 diklik
        Debug.Log("Button GameObjek2 diklik!");
    }

    IEnumerator SendTextRequest(string userText)
    {
        string jsonData = "{\"text\":\"" + userText + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error mengirim teks ke API: " + request.error);
        }
        else
        {
            Root responseData = JsonUtility.FromJson<Root>(request.downloadHandler.text);

            StartCoroutine(LoadImageFromUrl(responseData.imageUrl));

            originalTextUI.text = "Teks Asli: " + responseData.originalText;
            responseText.text = "Respon API: " + request.downloadHandler.text;
        }
    }

    IEnumerator LoadImageFromUrl(string imageUrl)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error memuat gambar: " + imageRequest.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);

            if (planeObject == null)
            {
                Debug.LogError("Objek pesawat tidak diassign!");
            }
            else if (planeObject.GetComponent<MeshRenderer>() == null)
            {
                Debug.LogError("Komponen MeshRenderer tidak ditemukan pada objek pesawat!");
            }
            else
            {
                SetObjectProperties();

                planeObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
            }
        }
    }

    void SetObjectProperties()
    {
        if (planeObject != null)
        {
            planeObject.transform.localScale = new Vector3(1f, 1f, 1f);
            planeObject.transform.position = new Vector3(0f, 0f, 2f);
            planeObject.transform.rotation = Quaternion.identity;
            planeObject.layer = LayerMask.NameToLayer("Default");

            MeshRenderer meshRenderer = planeObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && !meshRenderer.enabled)
            {
                meshRenderer.enabled = true;
            }
        }
    }
}
