﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool[,] roads;
    public HashSet<Vector2Int> destinationLocations;
    public static MapGenerator instance;

    int placedDestinations = 0;

    [Header("Map Settings")]
    public int gridSize;
    [SerializeField]
    int map_width = 97;
    [SerializeField]
    int map_height = 65;
    [SerializeField]
    int num_destinations = 6;
    

    [Header("Prefabs")]
    public List<GameObject> normiePrefabs;
    public List<GameObject> destinationPrefabs;
    public GameObject buildingPrefab;
    public GameObject sidewalkPrefab;
    public GameObject roadPrefab;

    [Header("Transform Parents")]
    public Transform roadsParent;
    public Transform buildingsParent;
    public Transform destinationsParent;
    public Transform trafficParent;

    [Header("Possible Textures")]
    public List<Texture2D> possibleCarTextures;

    int normieCount = 0;
    int targetNormieCount = 60;
    int maxNormieCount = 60;

    private void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        destinationLocations = new HashSet<Vector2Int>();

    }
    // Start is called before the first frame update
    void Start()
    {
        roads = new bool[map_width,map_height];

        for(int i = 0; i < map_width; i++)
        {
            for(int j = 0; j < map_height; j++)
            {
                if(i%gridSize == 0 || j%gridSize == 0)
                {
                    var rd = Instantiate(roadPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    rd.transform.SetParent(roadsParent);
                    roads[i, j] = true;


                    if (normieCount<maxNormieCount && Random.Range(0.0f, 1.0f) >= normieCount/targetNormieCount){
                        normieCount++;
                        int random = Random.Range(0, normiePrefabs.Count);
                        int randomColor = Random.Range(0, possibleCarTextures.Count);
                        var obj = Instantiate(normiePrefabs[random], new Vector3(i, 0, j), Quaternion.identity);
                        obj.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",possibleCarTextures[randomColor]);
                        obj.GetComponent<ThiefAI>().colorIndex = randomColor;
                        obj.GetComponent<ThiefAI>().vehicleIndex = random;
                        obj.transform.SetParent(trafficParent);
                    }
                }
                
            }
        }
        for(int ctr = 0; ctr < num_destinations; ctr++)
        {
            bool spotFound = false;
            while (!spotFound) {
                int randomI = Random.Range(0, map_width-1);
                int randomJ = Random.Range(0, map_height-1);
                //If only pick positions that are adjacent to roads, and are not roads
                if (!roads[randomI, randomJ] && (roads[randomI-1,randomJ] || roads[randomI+1,randomJ] 
                    || roads[randomI,randomJ+1] || roads[randomI,randomJ-1] || roads[randomI + 1, randomJ -1] 
                    || roads[randomI - 1,randomJ +1] || roads[randomI+1,randomJ+1] || roads[randomI-1,randomJ-1]))
                {
                    var dest = Instantiate(destinationPrefabs[placedDestinations], new Vector3(randomI, 0.1f, randomJ), Quaternion.identity);
                    placedDestinations = (placedDestinations + 1) % destinationPrefabs.Count;
                    dest.transform.SetParent(destinationsParent);
                    destinationLocations.Add(new Vector2Int(randomI, randomJ));
                    spotFound = true;
                }
            }
        }
        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                
                if (!roads[i,j] && !destinationLocations.Contains(new Vector2Int(i, j))) { 
                    var bld = Instantiate(buildingPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    bld.transform.SetParent(buildingsParent);
                }
                if (!roads[i,j]) { 
                    var sdwlk = Instantiate(sidewalkPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    sdwlk.transform.SetParent(buildingsParent);
                }

            }
        }
    }
}
