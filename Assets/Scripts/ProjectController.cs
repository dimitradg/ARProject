using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class ProjectController : MonoBehaviour
{
    

    public GameObject cubeToSpawn;
    public GameObject cubehost;
    public bool cubeSpawned = false;

    public GameObject tapIndicator;
    public Image uiIndicator;

    private Pose PlacementPose;
    private bool placementPoseValid = false;

    private ARRaycastManager arRayMngr;

    public string key;
    AsyncOperationHandle<GameObject> opHandle;

    void Start()
    {

        //initializing a RayCast Manager
        arRayMngr = FindObjectOfType<ARRaycastManager>();

        //defining Addressables Assets remote address
        key = "https://drive.google.com/drive/folders/1jfSGWhRwCe6W4wPFNmHTDWcc9IpVIgxp?usp=sharing";

    }

   
    void Update()
    {
        
        //checking if the user has touched the screen and if there is an AR object already placed
        if(!cubeSpawned && placementPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)

        {
            StartCoroutine(ARObectPlacement());
            cubeSpawned = true;

            UiController();   
        }

        
        UpdatePlacementPose();
        UpdateIndicator();
    }

    void UpdateIndicator()
    {
        //update the point where the "tap" indicator is placed during each frame
        if (!cubeSpawned && placementPoseValid)
        {
            
            tapIndicator.SetActive(true);
            tapIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
            
        }
        else
        {
            tapIndicator.SetActive(false);
        }
    }

    //update the center of the screen in relevance with the physical world captured by the ARcamera
    void UpdatePlacementPose()
    {
        //converting the 3D world into 2D and setting the center of our screen
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        //identifying planes in the physical world
        arRayMngr.Raycast(screenCenter, hits, TrackableType.Planes);

        //getting the first Raycast plane hit in each frame
        placementPoseValid = hits.Count > 0;

        //getting the position of the first Raycast plane hit
        if (placementPoseValid)
        {
            PlacementPose = hits[0].pose;

        }
    }

    public IEnumerator ARObectPlacement ()
    {
        //placing AR object as addressable from google drive
        
         
            opHandle = Addressables.LoadAssetAsync<GameObject>(key);
            yield return opHandle;

            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                cubehost = opHandle.Result;
                Instantiate(cubehost, transform);
            }
        
    }

    void UiController()
    {
        
        //getting the AR objects position and converting it into 2D dimentions in order to capture in on the UI screen
        Transform target = cubehost.transform;
        Vector3 Imagepos = Camera.main.WorldToScreenPoint(target.position);

        //changing the UI's indicator position in order to follow the AR object's position
        uiIndicator.GetComponent<RectTransform>().anchoredPosition = Imagepos;

    }
}
