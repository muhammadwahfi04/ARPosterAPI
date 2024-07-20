using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Place3D : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    [SerializeField]
    GameObject visualObject;

    [SerializeField]
    Text textObject1;

    [SerializeField]
    Text textObject2;

    [SerializeField]
    GameObject buttonObject;

    [SerializeField]
    GameObject sliderObject;

    [SerializeField]
    GameObject button2Object;

    [SerializeField]
    GameObject textFieldObject;

    UnityEvent placementUpdate;

    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    public GameObject spawnedObject { get; private set; }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();

        if (placementUpdate == null)
            placementUpdate = new UnityEvent();

        placementUpdate.AddListener(UpdateObjects);
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                placementUpdate.Invoke();

                // Set initial scale to one-fourth of the slider's maximum value
                float initialScale = 0.04f;
                sliderObject.GetComponent<Slider>().value = initialScale;
                UpdateScale(initialScale);

                // Disable visualObject when AR object appears
                visualObject.SetActive(false);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }

        // Check if the slider value has changed
        if (sliderObject.activeSelf && spawnedObject != null)
        {
            float scaleValue = sliderObject.GetComponent<Slider>().value;
            UpdateScale(scaleValue);
        }
    }

    void UpdateScale(float scaleValue)
    {
        Vector3 newScale = Vector3.one * scaleValue;
        spawnedObject.transform.localScale = newScale;
    }

    void UpdateObjects()
    {
        textObject1.text = "Place and size poster";
        textObject2.text = "Send";
        buttonObject.SetActive(true);
        sliderObject.SetActive(true); // Mengaktifkan game objek slider
    }

    public void OnButtonClick()
    {
        // Disable slider and change textObject1
        sliderObject.SetActive(false);
        textObject1.text = "Write your slogan";

        // Activate button2Object and textFieldObject
        button2Object.SetActive(true);
        textFieldObject.SetActive(true);
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
