using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool[,] roads;
    public Dictionary<Vector2Int, GameObject> destinations;
    public Dictionary<Vector2Int, GameObject> normalBuildings;
    public static MapGenerator instance;
    public GameSession gameSession;
    public List<GameObject> thieves;

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
    public string lastDestination;

    public int normieCount = 0;
    public int targetNormieCount = 60;
    public int maxNormieCount = 60;
    public TMP_Text bannerText;
    [SerializeField]
    public TMP_Text robbedAmountText;
    
    int robbedAmount;

    private void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        destinations = new Dictionary<Vector2Int, GameObject>();
        normalBuildings = new Dictionary<Vector2Int, GameObject>();
        bannerText.text = "Warning! Wanted robber on the loose, keep an eye on any high profile buildings!";
        robbedAmount = 0;
        robbedAmountText.text = "Total Damages: $0";
    }
    public void updateBanner(string robbedBuilding, bool isStealing)
    {
        bannerText.text = isStealing ? "Warning! The thief is currently robbing a " + robbedBuilding : "Warning! The thief has just stolen from a " + robbedBuilding;
    }
    public void addRobbedAmount()
    {
        robbedAmount += Random.Range(800, 1400);
        robbedAmountText.text = "Total Damages: $" + robbedAmount;
    }
    // Start is called before the first frame update
    void Start()
    {
        roads = new bool[map_width,map_height];
        bool placedCriminal = false;
        Vector3 lastSpawnLoc = new Vector3(0,0,0);
        for(int i = 0; i < map_width; i++)
        {
            for(int j = 0; j < map_height; j++)
            {
                if(i%gridSize == 0 || j%gridSize == 0)
                {
                    var rd = Instantiate(roadPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    rd.transform.SetParent(roadsParent);
                    roads[i, j] = true;


                    if (normieCount<maxNormieCount && (Random.Range(0.0f, 1.0f) >= normieCount/targetNormieCount) && Vector3.Distance(new Vector3(i,0,j),lastSpawnLoc) > 2.5f){
                        int random = Random.Range(0, normiePrefabs.Count);
                        normieCount++;
                        int randomColor = Random.Range(0, possibleCarTextures.Count);
                        lastSpawnLoc = new Vector3(i, 0, j);
                        var obj = Instantiate(normiePrefabs[random], lastSpawnLoc, Quaternion.identity);
                        obj.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",possibleCarTextures[randomColor]);
                        obj.GetComponentInChildren<ThiefAI>().colorIndex = randomColor;
                        obj.GetComponentInChildren<ThiefAI>().vehicleIndex = random;
                        obj.GetComponentInChildren<ThiefAI>().speed *= Random.Range(0.8f, 1.2f);
                        obj.transform.SetParent(trafficParent);
                        if(!placedCriminal && random != 1){
                            obj.GetComponentInChildren<ThiefAI>().isThief = true;
                            placedCriminal = true;
                            obj.transform.localScale = new Vector3(1, 1, 1);
                            gameSession.currentCriminal = obj.GetComponentInChildren<ThiefAI>();
                            thieves.Add(obj.gameObject);
                        }
                        if(random == 1){
                            for(int a = 0; a < 5; a++){
                                randomColor = Random.Range(0, possibleCarTextures.Count);
                                lastSpawnLoc = new Vector3(i, 0, j);
                                var obj2 = Instantiate(normiePrefabs[random], lastSpawnLoc, Quaternion.identity);
                                obj2.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",possibleCarTextures[randomColor]);
                                obj2.GetComponentInChildren<ThiefAI>().colorIndex = randomColor;
                                obj2.GetComponentInChildren<ThiefAI>().vehicleIndex = random;
                                obj2.GetComponentInChildren<ThiefAI>().speed *= Random.Range(0.8f, 1.2f);
                                obj2.transform.SetParent(trafficParent);
                            }
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
                    bool tooClose = false;
                    foreach(var destination in destinations)
                    {
                        if(Vector2Int.Distance(destination.Key,pos) < 4f)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if(tooClose)
                    {
                        continue;
                    }
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
                    normalBuildings.Add(new Vector2Int(i, j), bld);
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
