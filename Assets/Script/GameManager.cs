using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
     
    public DragGameSprite Spectrobe;
    public GameObject SpectrobeRenderer;
    public GameObject Mineral;
    public List<GameObject> currentSpectrobes;
    public List<GameObject> currentMinerals;
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
        // Set a fixed depth for spawning. Here, we assume the camera is at a negative z and looking toward positive z.
        float spawnDepth = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 screenPos = new Vector3(randomX, randomY, spawnDepth);

        // Convert the screen position to a world position.
        Vector3 spawnPos = mainCamera.ScreenToWorldPoint(screenPos);

        // Instantiate the Mineral prefab at the calculated position.
        GameObject spawnedMineral = Instantiate(Mineral, spawnPos, Quaternion.identity);
        currentMinerals.Add(spawnedMineral);
        // Optionally add the SpectrobeRenderer to the ScreenSpaceCollisionDetector.
        ScreenSpaceCollisionDetector detector = spawnedMineral.GetComponentInChildren<ScreenSpaceCollisionDetector>();
        if (detector != null)
        {
            foreach (GameObject spec in currentSpectrobes)
            {
                detector.otherObjects.Add(spec.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
            }
        }

        // Replace the mesh with a random one from mineralMeshes.
        MeshFilter meshFilter = spawnedMineral.GetComponentInChildren<MeshFilter>();
        Mesh newMesh = null;
        if (meshFilter != null && mineralMeshes.Count > 0)
        {
            int randomMeshIndex = Random.Range(0, mineralMeshes.Count);
            newMesh = mineralMeshes[randomMeshIndex];
            meshFilter.mesh = newMesh;
        }

        // Replace the material(s) based on the mesh's submesh count.
        Renderer rend = spawnedMineral.GetComponentInChildren<Renderer>();
        if (rend != null && mineralMats.Count > 0)
        {
            // Determine submesh count from the new mesh. If no mesh was assigned, assume 1.
            int subMeshCount = (newMesh != null) ? newMesh.subMeshCount : 1;
            // Create a new material array of the appropriate size.
            Material[] newMats = new Material[subMeshCount];

            if (subMeshCount == 1)
            {
                newMats[0] = mineralMats[Random.Range(0, mineralMats.Count)];
            }
            else if (subMeshCount >= 2)
            {
                // For two or more submeshes, assign two random materials to the first two slots.
                newMats[0] = mineralMats[Random.Range(0, mineralMats.Count)];
                newMats[1] = mineralMats[Random.Range(0, mineralMats.Count)];
                // For any additional submeshes, fill the rest with a random material.
                for (int i = 2; i < subMeshCount; i++)
                {
                    newMats[i] = mineralMats[Random.Range(0, mineralMats.Count)];
                }
            }
            rend.materials = newMats;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
