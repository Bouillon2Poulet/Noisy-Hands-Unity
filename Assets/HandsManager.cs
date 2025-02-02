using System;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;

public class HandsManager : MonoBehaviour
{
    public class Hand3D
    {
        public string handedness;
        public Vector3 position;
        public float radius;

        public GameObject sphere;

        public Hand3D(string handedness, Vector3 position, float radius)
        {
            this.handedness = handedness;
            this.position = position;
            this.radius = radius;

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Color sphereColor = this.handedness == "Right" ? Color.red : Color.blue;
            sphere.transform.localScale = new Vector3(100, 100, 100);
            sphere.GetComponent<Renderer>().material.color = sphereColor;
            this.sphere = sphere;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = new Vector3((position.x - 0.5f) * -1280, 100, (position.y - 0.5f) * 720);
        }

        public void UpdatePosition()
        {
            sphere.transform.position = position;
        }

        public void SetScale(Vector2 scale)
        {
            radius = Math.Max(scale.x, scale.y);
        }

        public void UpdateScale()
        {
            sphere.transform.localScale = new Vector3(100, 100, 100) * radius;
        }

        public void Update()
        {
            UpdatePosition();
            UpdateScale();
        }
    }

    public Hand3D _rightHand;
    public Hand3D _leftHand;

    void Start()
    {
        _rightHand = new Hand3D("Right", new Vector3(0, 0, 0), 1.0f);
        _leftHand = new Hand3D("Left", new Vector3(0, 0, 0), 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        _rightHand.Update();
        _leftHand.Update();
    }

    public void UpdateHands(HandLandmarkerResult result)
    {
        if (result.handedness == null) { return; }
        for (int i = 0; i < result.handedness.Count; i++)
        {
            Hand3D targetHand = null;
            switch (result.handedness[i].categories[0].displayName)
            {
                case "Right": targetHand = _rightHand; break;
                case "Left": targetHand = _leftHand; break;
                default: Debug.Log("No Hands detected"); return;
            }

            if (targetHand == null)
            {
                Debug.LogError("Target hand is null, something went wrong!");
                return;
            }

            Vector3 positionSum = Vector3.zero;
            float xMin = result.handLandmarks[i].landmarks[0].x;
            float xMax = result.handLandmarks[i].landmarks[0].x;
            float yMin = result.handLandmarks[i].landmarks[0].y;
            float yMax = result.handLandmarks[i].landmarks[0].y;

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
            }
            targetHand.SetPosition(positionSum / result.handLandmarks[i].landmarks.Count);
            targetHand.SetScale(new Vector2(xMax - xMin, yMax - yMin));
        }
        Debug.Log($"Right Hand Position: {_rightHand.position}");
        Debug.Log($"Left Hand Position: {_leftHand.position}");
    }
}
