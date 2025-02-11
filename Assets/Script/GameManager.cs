using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DragGameSprite Spectrobe;
    public GameObject SpectrobeRenderer;
    public GameObject Mineral;
    public List<Mesh> mineralMeshes;
    public List<Material> mineralMats;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;   
    }

    public void SpawnMineral()
    {
        // Choose a random screen position.
        float randomX = Random.Range(0, Screen.width);
        float randomY = Random.Range(0, Screen.height);
        // Set a fixed depth for spawning. Here, we use the absolute value of the camera's z position,
        // assuming the camera is positioned at a negative z value and looking toward positive z.
        float spawnDepth = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 screenPos = new Vector3(randomX, randomY, spawnDepth);

        // Convert the screen position to a world position.
        Vector3 spawnPos = mainCamera.ScreenToWorldPoint(screenPos);

        // Instantiate the Mineral prefab at the calculated position.
        GameObject spawnedMineral = Instantiate(Mineral, spawnPos, Quaternion.identity);
        spawnedMineral.GetComponentInChildren<ScreenSpaceCollisionDetector>().otherObjects.Add(SpectrobeRenderer);
        
        // Replace the mesh with a random one from mineralMeshes.
        MeshFilter meshFilter = spawnedMineral.GetComponentInChildren<MeshFilter>();
        if (meshFilter != null && mineralMeshes.Count > 0)
        {
            int randomMeshIndex = Random.Range(0, mineralMeshes.Count);
            meshFilter.mesh = mineralMeshes[randomMeshIndex];
        }

        // Replace the material(s) with random material(s) from mineralMats.
        Renderer rend = spawnedMineral.GetComponentInChildren<Renderer>();
        if (rend != null && mineralMats.Count > 0)
        {
            Material[] mats = rend.materials;
            if (mats.Length == 1)
            {
                // For one material, pick a random material.
                int randomMatIndex = Random.Range(0, mineralMats.Count);
                mats[0] = mineralMats[randomMatIndex];
            }
            else if (mats.Length >= 2)
            {
                // For two or more materials, pick two random materials for the first two slots.
                int randomMatIndex1 = Random.Range(0, mineralMats.Count);
                int randomMatIndex2 = Random.Range(0, mineralMats.Count);
                mats[0] = mineralMats[randomMatIndex1];
                mats[1] = mineralMats[randomMatIndex2];
            }
            rend.materials = mats;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
