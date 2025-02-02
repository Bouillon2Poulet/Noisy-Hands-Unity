using System.Collections.Generic;
using UnityEngine;

public class GardenManager : MonoBehaviour
{
    float _flowerBaseScale = 200;
    float _flowerDetectionRadius = 200;

    [SerializeField] private SunBehavior _sun;
    [SerializeField] private GameObject _flowerPrefab;


    List<Flower> Flowers;
    class Flower
    {
        public Vector3 position;
        public float scale;
        private GameObject mesh;

        public Flower(Vector3 position, float baseScale, GameObject flowerPrefab)
        {
            this.position = position;
            GameObject flowerInstance = Instantiate(flowerPrefab, this.position, Quaternion.identity);
            Color sphereColor = Color.yellow;
            flowerInstance.transform.localScale *= baseScale;
            // flowerInstance.GetComponent<Renderer>().material.color = sphereColor;
            mesh = flowerInstance;
            mesh.transform.position = this.position;
        }
        public void setScale(float newScale)
        {
            scale = newScale;
            mesh.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int nbFlower = 30;
        float rowOffset = 1280 / nbFlower;
        float rowStartOffset = rowOffset / 2;
        float columnOffset = 720 / nbFlower;
        float columnStartOffset = columnOffset / 2;

        Flowers = new List<Flower>();

        for (int i = 0; i < nbFlower; i++)
        {
            for (int j = 0; j < nbFlower; j++)
            {
                float offsetX = Random.Range(-rowOffset / 4, rowStartOffset / 4);
                float offsetY = Random.Range(-columnOffset / 4, columnOffset / 4);
                Flowers.Add(new Flower(new Vector3(rowStartOffset + (i * rowOffset - 1280 / 2) + offsetX, 0, columnStartOffset + (j * columnOffset - 720 / 2)), _flowerBaseScale + offsetY, _flowerPrefab));
                Debug.Log("New Flower !");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sunPosition = _sun._sunPosition;
        Vector3 localSunDirection = new Vector3(0, -1, -1).normalized;
        Vector3 sunDirection = _sun.transform.TransformDirection(localSunDirection);
        Vector3 gardenNormal = new Vector3(0, 1, 0);
        Debug.Log($"sunDirection: {sunDirection}");
        Vector3 intersectionPosition = new Vector3();

        _flowerDetectionRadius = _sun._sunRadius * 1.5f;
        if (CalculateIntersection(new Vector3(0, 0, 0), gardenNormal, sunPosition, sunDirection, ref intersectionPosition))
        {
            foreach (Flower flower in Flowers)
            {
                Vector3 delta = flower.position - intersectionPosition;
                delta.y = 0;
                float distance = delta.magnitude;
                float scale = distance > _flowerDetectionRadius ? 0 : 1 - distance / _flowerDetectionRadius;
                flower.setScale(scale * _flowerBaseScale);
            }
        }
    }

    public static bool CalculateIntersection(Vector3 planePoint, Vector3 planeNormal, Vector3 lineOrigin, Vector3 lineDirection, ref Vector3 intersectionPosition)
    {
        float denom = Vector3.Dot(planeNormal, lineDirection);

        if (Mathf.Abs(denom) < Mathf.Epsilon)
        {
            return false; // Pas d'intersection ou droite dans le plan
        }

        float t = Vector3.Dot(planeNormal, planePoint - lineOrigin) / denom;
        intersectionPosition = lineOrigin + t * lineDirection;
        return true;
    }
}
