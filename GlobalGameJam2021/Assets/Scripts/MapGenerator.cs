using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool[,] roads;
    public Dictionary<Vector2Int, GameObject> destinations;
    public static MapGenerator instance;
    public GameSession gameSession;

    int placedDestinations = 0;

    [Header("Map Settings")]
    public int gridSize;
    public int map_width = 17;
    public int map_height = 11;
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
    public Material red;
    public GameObject newsFlare;

    public int normieCount = 0;
    public int targetNormieCount = 60;
    public int maxNormieCount = 60;

    private void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        destinations = new Dictionary<Vector2Int, GameObject>();

    }
    // Start is called before the first frame update
    void Start()
    {
        roads = new bool[map_width,map_height];
        bool placedCriminal = false;

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
                        if(!placedCriminal){
                            obj.GetComponent<ThiefAI>().isThief = true;
                            placedCriminal = true;
                            obj.transform.localScale = new Vector3(1, 1, 1);
                            gameSession.currentCriminal = obj.GetComponent<ThiefAI>();
                        }
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
                Vector2Int pos = new Vector2Int(randomI, randomJ);
                //If only pick positions that are adjacent to roads, and are not roads
                if ((!roads[randomI, randomJ] && (roads[randomI-1,randomJ] || roads[randomI+1,randomJ] 
                    || roads[randomI,randomJ+1] || roads[randomI,randomJ-1] || roads[randomI + 1, randomJ -1] 
                    || roads[randomI - 1,randomJ +1] || roads[randomI+1,randomJ+1] || roads[randomI-1,randomJ-1])) && !destinations.ContainsKey(pos))
                {
                    var dest = Instantiate(destinationPrefabs[placedDestinations], new Vector3(randomI, 0.1f, randomJ), Quaternion.identity);
                    placedDestinations = (placedDestinations + 1) % destinationPrefabs.Count;
                    
                    dest.transform.SetParent(destinationsParent);
                    destinations.Add(pos,dest);
                    spotFound = true;
                }
            }
        }
        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                
                if (!roads[i,j] && !destinations.ContainsKey(new Vector2Int(i, j))) { 
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
