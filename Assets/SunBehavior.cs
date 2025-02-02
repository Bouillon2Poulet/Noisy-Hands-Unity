using System;
using UnityEngine;

public class SunBehavior : MonoBehaviour
{
    [SerializeField] private HandsManager _handsManager;

    public float _sunRotation = 0;
    public float _sunRadius = 0;
    public Vector3 _sunPosition = new Vector3(0, 0, 0);
    GameObject _sunLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sunLight = GameObject.Find("Cone");
    }

    // Update is called once per frame
    void Update()
    {
        if (_handsManager == null)
        {
            return;
        }
        _sunRotation = _handsManager._leftSideHand.rotationAngle;
        _sunPosition = _handsManager._rightSideHand.position;
        _sunRadius = _handsManager._rightSideHand.radius;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _sunRotation, transform.eulerAngles.z);
        transform.position = _sunPosition;
        float radiusMinValue = 100;
        float radiusMaxValue = 200;
        float currentRadius = Math.Min(radiusMinValue + _sunRadius * (radiusMaxValue - radiusMinValue), radiusMaxValue);
        _sunRadius = currentRadius;
        _sunLight.transform.localScale = new Vector3(currentRadius, currentRadius, 100);
    }
}
