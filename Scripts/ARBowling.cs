
//namespace
namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

   
    /// Controls the HelloAR example.
    
    public class ARBowling : MonoBehaviour
    {
        //maximum distance player-ball
        public float distancePlayerBall = 0.75f;

        //set pins trought script
        public Pins[] pins;

        //for spawing only one object
        public bool spawn = false;

        //give some time to ready the ball creation
        public bool ready = false;

        //for creating the ball
        public bool bowlingBallCreated = false;

        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        
        public Camera FirstPersonCamera;

       
        /// A prefab for tracking and visualizing detected planes.
        
        public GameObject DetectedPlanePrefab;

       
        /// A model to place when a raycast from a user touch hits a plane.
        public GameObject bowlingEnvironment;

        //the model used for the ball of the bowling
        public GameObject bowlingBallPrefab;
       
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        
        private const float k_ModelRotation = 180.0f;

        //this is the score manager
        public Score scoreManag;
       
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        
        private bool m_IsQuitting = false;

        //this if for the ball bowling gameobject
        GameObject ballGo = null;
        Touch touch;

        //used to calculate the speed
        public Follower referenceFollower;
        Vector3 pos;
        Rigidbody _rb;

        public float speedFactor=50;

        /// The Unity Update() method.

        private void Start()
        {
            //Invoke("checkPins", 5);

            pos = Camera.main.transform.position;
        }

        public void FixedUpdate()
        {
            _UpdateApplicationLifecycle();


            //////////////////////////////////////////////
            //this the bowling ball gameobject interaction
            /////////////////////////////////////////////
            if ((Input.touchCount == 1 && spawn == true && ready==true))
            {
                touch = Input.GetTouch(0);

                //use input of the touch for creating the position of the ball use first line in editor

                //use raycast against the ground object to know where to move the ball this works in a distance of 1m from the player only
                RaycastHit hit2;
                
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(touch.position.x, touch.position.y));
                if (Physics.Raycast(ray, out hit2, 300))
                {
                    if (hit2.collider.gameObject.tag == "ground")
                    {
                        pos = hit2.point + 0.01f * hit2.collider.gameObject.transform.up;
                    }

                }

                


                if (bowlingBallCreated == false && touch.phase == TouchPhase.Began)
                {
                    bowlingBallCreated = true;

                    //instantiate the prefab

                    ballGo =GameObject.Instantiate(bowlingBallPrefab, pos, Quaternion.Euler(Random.Range(0, 0), Random.Range(0, 0), Random.Range(0, 0)));
                    ballGo.transform.position = pos;
                    
                    //disable collider when moving
                    ballGo.GetComponent<Collider>().enabled = false;

                    referenceFollower.t = ballGo.transform;
                    _rb = ballGo.GetComponent<Rigidbody>();
                    _rb.useGravity = false;

                }
                //while moving the ball
                else if (touch.phase == TouchPhase.Moved)
                {
                    if ((Camera.main.transform.position-pos).magnitude< distancePlayerBall)
                    {
                       
                        //move rigidbody
                        _rb.MovePosition(pos);
                    }
                }
                //when trhowing the ball
                else if (touch.phase == TouchPhase.Ended && ballGo != null)
                {
                    bowlingBallCreated = false;

                    //move rigidbody
                    _rb.MovePosition(pos);

                    //enable collider when release
                    ballGo.GetComponent<Collider>().enabled = true;
                    _rb.useGravity = true;

                    Vector3 speed = (pos - referenceFollower.transform.position)* speedFactor;


                    _rb.velocity = speed;


                    referenceFollower.t = null;

                    _rb.useGravity = true;


                    ready = false;

                    Invoke("checkPins", 10);                  
                    Invoke("setReady", 13);
                    Destroy(ballGo, 10);
                    //ballGo.transform.GetComponent<Rigidbody>().velocity= FirstPersonCamera.transform.forward*1;

                }


            }

            
            // If the player has not touched the screen, we are done with this update.
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began || spawn==true)
            {
                return;
            }
       
            
            // Should not handle input if the player is pointing on UI.
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    // Choose the Andy model for the Trackable that got hit.
                    GameObject prefab;
                    if (hit.Trackable is FeaturePoint)
                    {
                        prefab = bowlingEnvironment;
                    }
                    else
                    {
                        prefab = bowlingEnvironment;
                    }

                    // Instantiate Andy model at the hit pose only if it is not spawned

                    Vector3 directionForPRefab = Camera.main.transform.right;
                    directionForPRefab = new Vector3(directionForPRefab.x,0,directionForPRefab.z);

                    var andyObject = prefab;//Instantiate(prefab, hit.Pose.position, Quaternion.Euler(0,0,0));


                    //set initial position and rotation
                    andyObject.transform.position = hit.Pose.position;
                    andyObject.transform.rotation = Quaternion.Euler(0, 0, 0);

                    andyObject.transform.right = -directionForPRefab;

                    //set spawn to true
                    spawn = true;

                    //ready function call
                    Invoke("setReady", 1);

                    
                    // Compensate for the hitPose rotation facing away from the raycast (i.e.
                    // camera).
                    andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    // Make Andy model a child of the anchor.
                    andyObject.transform.parent = anchor.transform;


                    //enable gameobject
                    andyObject.SetActive(true);

                }
            }
        }

       //ready function
       public void setReady()
        {
            ready = true;
        }

        //ready function
        public void checkPins()
        {

            scoreManag.updateFrame();

        }


  

        /// Check and update the application lifecycle.

        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

       
        /// Actually quit the application.
        
        private void _DoQuit()
        {
            Application.Quit();
        }

       
        /// Show an Android toast message.
        
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
