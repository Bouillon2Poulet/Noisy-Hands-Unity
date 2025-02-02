using System;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;

public class HandsManager : MonoBehaviour
{
    [SerializeField] private GameObject _debugMeshPrefab;
    public class Hand3D
    {
        public string handedness;
        public Vector3 position;
        public float radius;
        public float rotationAngle = 0;

        public GameObject debugMesh;

        public Hand3D(string handedness, Vector3 position, float radius, GameObject debugMeshPrefab)
        {
            this.handedness = handedness;
            this.position = position;
            this.radius = radius;

            GameObject debugMeshInstance = Instantiate(debugMeshPrefab, this.position, Quaternion.identity);
            debugMeshInstance.transform.localScale = new Vector3(100, 100, 100);
            debugMesh = debugMeshInstance;
        }

        public void SetPosition(Vector3 position)
        {
            Debug.Log(position);
            this.position = new Vector3((position.x - 0.5f) * 1280, 300, (position.y - 0.5f) * -720);
        }

        public void UpdatePosition()
        {
            debugMesh.transform.position = position;
        }

        public void SetScale(Vector2 scale)
        {
            radius = Math.Max(scale.x, scale.y);
        }

        public void UpdateScale()
        {
            debugMesh.transform.localScale = new Vector3(100, 100, 100) * radius;
        }

        public void SetRotation(float rotationAngle)
        {
            this.rotationAngle = rotationAngle;
            Debug.Log(this.rotationAngle);
        }

        public void UpdateRotation()
        {
            debugMesh.transform.eulerAngles = new Vector3(debugMesh.transform.eulerAngles.x, rotationAngle, debugMesh.transform.eulerAngles.z);
        }

        public void UpdateMeshes()
        {
            UpdatePosition();
            UpdateScale();
            UpdateRotation();
        }
    }

    public Hand3D _rightSideHand;
    public Hand3D _leftSideHand;
    [SerializeField] private bool _displayDebugMeshes = false;

    void Start()
    {
        _rightSideHand = new Hand3D("Right", new Vector3(0, 0, 0), 1.0f, _debugMeshPrefab);
        _leftSideHand = new Hand3D("Left", new Vector3(0, 0, 0), 1.0f, _debugMeshPrefab);

        if (_rightSideHand == null)
        {
            Debug.Log("RIGHT HAND NULL");
        }
        // if (!_displayDebugMeshes)
        // {
        //     _rightSideHand.debugMesh.SetActive(false);
        //     _leftSideHand.debugMesh.SetActive(false);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (_displayDebugMeshes)
        {
            _rightSideHand.UpdateMeshes();
            _leftSideHand.UpdateMeshes();
        }
    }

    public void UpdateHands(HandLandmarkerResult result)
    {
        if (result.handedness == null) { return; }
        for (int i = 0; i < result.handedness.Count; i++)
        {
            // switch (result.handedness[i].categories[0].displayName)
            // {
            //     case "Right": targetHand = _rightSideHand; break;
            //     case "Left": targetHand = _leftSideHand; break;
            //     default: Debug.Log("No Hands detected"); return;
            // }

            // if (targetHand == null)
            // {
            //     Debug.LogError("Target hand is null, something went wrong!");
            //     return;
            // }

            Vector3 positionSum = Vector3.zero;
            float xMin = result.handLandmarks[i].landmarks[0].x;
            float xMax = result.handLandmarks[i].landmarks[0].x;
            float yMin = result.handLandmarks[i].landmarks[0].y;
            float yMax = result.handLandmarks[i].landmarks[0].y;

            Vector3 thumbTipPosition = Vector3.zero;
            Vector3 indexFingerTipPosition = Vector3.zero;

            for (int j = 0; j < result.handLandmarks[i].landmarks.Count; j++)
            {
                Vector3 landmarkPosition = new Vector3(result.handLandmarks[i].landmarks[j].x, result.handLandmarks[i].landmarks[j].y, result.handLandmarks[i].landmarks[j].z);
                if (landmarkPosition.x > xMax)
                {
                    xMax = landmarkPosition.x;
                }
                else if (landmarkPosition.x < xMin)
                {
                    xMin = landmarkPosition.x;
                }

                if (landmarkPosition.y > yMax)
                {
                    yMax = landmarkPosition.y;
                }
                else if (landmarkPosition.y < yMin)
                {
                    yMin = landmarkPosition.y;
                }

                positionSum += landmarkPosition;

                if (j == 4)
                {
                    thumbTipPosition = landmarkPosition;
                }

                else if (j == 8)
                {
                    indexFingerTipPosition = landmarkPosition;
                }
            }
            Vector3 averagePosition = positionSum / result.handLandmarks[i].landmarks.Count;
            Hand3D targetHand = null;

            if (averagePosition.x < 0.5)
            {
                Debug.Log("Left side !");
                targetHand = _leftSideHand;
            }
            else if (averagePosition.x >= 0.5)
            {
                Debug.Log("Right side !");
                targetHand = _rightSideHand;
            }

            if (targetHand == null)
            {
                Debug.Log("Null :(");
                return;
            }
            targetHand.SetPosition(averagePosition);
            targetHand.SetScale(new Vector2(xMax - xMin, yMax - yMin));
            Vector2 thumbsToIndexVector = new Vector2((indexFingerTipPosition - thumbTipPosition).x, (indexFingerTipPosition - thumbTipPosition).y);
            targetHand.SetRotation(-Vector2.SignedAngle(thumbsToIndexVector.normalized, new Vector2(0, 1)));

        }
        Debug.Log($"Right Hand Position: {_rightSideHand.position}");
        Debug.Log($"Left Hand Position: {_leftSideHand.position}");
    }
}
