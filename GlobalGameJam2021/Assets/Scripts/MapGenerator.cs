using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    int map_width = 97;
    [SerializeField]
    int map_height = 65;
    [SerializeField]
    int num_destinations = 6;
    public bool[,] roads;
    [SerializeField]
    GameObject roadPrefab;
    [SerializeField]
    GameObject destinationPrefab;
    public HashSet<Vector2Int> destinationLocations;
    public static MapGenerator instance;
    private void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        roads = new bool[map_width,map_height];
        destinationLocations = new HashSet<Vector2Int>();
        for(int i = 0; i < map_width; i++)
        {
            for(int j = 0; j < map_height; j++)
            {
                if(i%16 == 0 || j%16 == 0)
                {
                    Instantiate(roadPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    roads[i, j] = true;
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
                    Instantiate(destinationPrefab, new Vector3(randomI, 0.1f, randomJ), Quaternion.identity);
                    destinationLocations.Add(new Vector2Int(randomI, randomJ));
                    spotFound = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
