using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    private Transform spawnPoint;
    
    //Array dimensions
    private int tileArraySizeX = 14;
    private int tileArraySizeY = 10;

    //Amount of empty blocks made for game level
    private int amountToAdd = 5;
    private int addBlockCounter;

    //Tile Object dimensions
    public Vector3 tileSize;

    //gameObject to instantiate, I have it set to public so that it is changeable in the editor and I can test dimensional changes in various different object -- purely to debug
    public GameObject tileObject;
    //Player spawn instantiation WILL CHANGE THIS TO Resources.Load() folder eventually
    public GameObject playerSpawnPrefab;


    //Tile List
    private GameObject[,] tilePlacement2dArray;
    //Tile Solid List
    public List<GameObject> solidTileList = new List<GameObject>();
    //Tile Solid List selected object ID and the GameObject container it is assigned to
    public int solidListID; //MUST BE CHANGED TO PRIVATE FOR DEPLOY---------------------------------!!!!!!!!!!!!
    private GameObject solidListSelectedObj;
    private GameObject posInMainTileArray;

    //Tile Checker List
    public List<GameObject> tileCheckerList = new List<GameObject>();
    //Tile Checker reference object
    private GameObject spawnSelector;

    //Exclusion List
    public List<GameObject> tileExclusionList = new List<GameObject>();

    //Y IS HORIZONTAL, X VERTICAL
    //Temp coordinate for tile/array
    private int coordX;
    private int coordY;
    private int solidCoordX;
    private int solidCoordY;

    //Material cache
    public Material standardBlockMaterial;
    public Material transparentMaterial;


    void Start()
    {
    spawnPoint = GameObject.Find("MapGeneratorOriginObj").transform; //Finds the transform of the "Spawnpoint" gameobject object
    tileSize = tileObject.gameObject.renderer.bounds.extents * 2; //adds the tile objects dimesions to the tileSize value
    tilePlacement2dArray = new GameObject[tileArraySizeX, tileArraySizeY]; //makes a new array of gameobjects with dimensions based on 2 "x,y" variables
    
    //tileSettings2dArray = new string[tileArraySizeX, tileArraySizeY]; //makes the array of gameObject tile awareness settings
    
    CreateTileGrid();
    AddBorderToExclusionList();
    //GetRandomTile();
    AddBlocks();
    SetPlayerSpawn();
    }


    void Update()
    {

    }



    //This is the loop that creates the array of gameobjects, the "public gameobject" part of it is the returned datatype
    public void CreateTileGrid()//The parameter is a string that identifies the origin point of the grid building.
    {
        
        GameObject tileParent = new GameObject(); //Sets a parent object for the overal grid, all tiles instanced in the loop below will be a child of this object.
        tileParent.gameObject.name = "tileGridParent";
        tileParent.transform.position = spawnPoint.transform.position;
        tileParent.transform.rotation = spawnPoint.transform.rotation;

        for (int x = 0; x < tileArraySizeX; x++)
        {
            for (int y = 0; y < tileArraySizeY; y++)
            {
                GameObject tileInstance = (GameObject)Instantiate(tileObject, new Vector3(100, 100, 100)/*Arbitrary, spawn out of view*/, Quaternion.identity);
                tileInstance.name = ("tileGridRef: " + x +"," + y);
                tileInstance.transform.parent = tileParent.transform;
                tileInstance.transform.position = new Vector3(
                                                        spawnPoint.position.x + x * -tileSize.x,
                                                        spawnPoint.position.y + y * -tileSize.y,
                                                        spawnPoint.position.z);

                tileInstance.gameObject.GetComponent<MeshRenderer>().enabled = false;
                tilePlacement2dArray[x, y] = tileInstance; //this is going to add each tile to an array using the correct coordinates

                //GetRandomTile();
            }

        }


    }

    void AddBorderToExclusionList()
    {
        //Adds all of the border squares to the exclusion list

        for (int x = 0; x < tileArraySizeX; x++)
        {
            GameObject tempTile = tilePlacement2dArray[x,0];
            tileExclusionList.Add(tempTile);
            //Debug.Log(tempTile);
            tempTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
            tempTile.gameObject.renderer.material.color = Color.red;
            tempTile.gameObject.tag = "borderTile";
        }
        for (int x = 0; x < tileArraySizeY; x++)
        {
            GameObject tempTile = tilePlacement2dArray[0, x];
            tileExclusionList.Add(tempTile);
            //Debug.Log(tempTile);
            tempTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
            tempTile.gameObject.renderer.material.color = Color.red;
            tempTile.gameObject.tag = "borderTile";
        }
        for (int x = 0; x < tileArraySizeX; x++)
        {
            GameObject tempTile = tilePlacement2dArray[x, tileArraySizeY -1];
            tileExclusionList.Add(tempTile);
            //Debug.Log(tempTile);
            tempTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
            tempTile.gameObject.renderer.material.color = Color.red;
            tempTile.gameObject.tag = "borderTile";
        }
        for (int x = 0; x < tileArraySizeY; x++)
        {
            GameObject tempTile = tilePlacement2dArray[tileArraySizeX - 1, x];
            tileExclusionList.Add(tempTile);
            //Debug.Log(tempTile);
            tempTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
            tempTile.gameObject.renderer.material.color = Color.red;
            tempTile.gameObject.tag = "borderTile";
        }

    }
   
   void GetRandomTile()
    {
        coordX = Random.Range(1, tileArraySizeX);
        coordY = Random.Range(1, tileArraySizeY);
        spawnSelector = tilePlacement2dArray[coordX, coordY];
        if(spawnSelector.gameObject.tag == "borderTile") //Makes sure the coords never output border tiles
        {
            GetRandomTile();
        }
        //Debug.Log(coordX);
        //Debug.Log(coordY);
    }

    void GetRandomSolidTile() //This is specifically for finding a solid block after the level generation is complete.
   {
        solidListID = Random.Range(0, solidTileList.Count);
        solidListSelectedObj = solidTileList[solidListID];
        Debug.Log("Solid LIST ID" + solidListID);
        Debug.Log("Solid tile selected" + solidListSelectedObj);

        //Debug.Log(solidListSelectedObj.gameObject.name);


        //solidCoordX = listPositionID / 10 % 10; 
        //This will get a seperate int using the remainder after division by 10
        //solidCoordY = listPositionID % 10; 
        //Same process, increase the division number by adding a 0 for every additional digit length of an integer. EG: 256 would be split to its 3rd digit by dividing by 100, 3495 would be divided by 1000

        /*
        int listPositionID = tileExclusionList.IndexOf(solidListSelectedObj);
        solidCoordX = listPositionID / 10 % 10; //This will get a seperate int using the remainder after division by 10
        solidCoordY = listPositionID % 10; //Same process, increase the division number by adding a 0 for every additional digit length of an integer. EG: 256 would be split to its 3rd digit by dividing by 100, 3495 would be divided by 1000
        Debug.Log(listPositionID);
        Debug.Log(solidCoordX);
        Debug.Log(solidCoordY);
       //SpawnBlockCheck(solidCoordX, solidCoordY);
        */
   }


    public void AddBlocks()//If list does not contain the item that the coords point to then turn off mesh and collider
    {
        for (int x = 0; x < amountToAdd; x++)
        {
            GetRandomTile();
            GameObject tileSelector = tilePlacement2dArray[coordX, coordY];
            if (tileSelector.gameObject.tag == "emptyTile")
            {
                setSolidSquare(tileSelector);
                x++;
            }
            GetSurroundingWallCount(coordX, coordY);
            x--;
        }
    }

 //Made setSolidSquare a seperate function so I could check against different tag names and send the tile through it.
    void setSolidSquare(GameObject tileToSet)
    {
        tileExclusionList.Add(tileToSet);
        tileToSet.gameObject.tag = "solidTile";
        solidTileList.Add(tileToSet);
        tileToSet.gameObject.GetComponent<MeshRenderer>().enabled = true;
        //Debug.Log(tileToSet);
        Debug.Log(addBlockCounter);
        addBlockCounter++;
    }


    void SetPlayerSpawn()
    {
        Debug.Log("MAKE SPAWN");
        GetRandomSolidTile();

        GameObject spawnBaseTile = tilePlacement2dArray[solidCoordX + 1, solidCoordY];
            spawnBaseTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
            spawnBaseTile.gameObject.renderer.material = transparentMaterial;
        if (spawnBaseTile.gameObject.tag == "emptyTile")
        {
            GameObject playerSpawnInstance = (GameObject)Instantiate(playerSpawnPrefab, spawnBaseTile.transform.position, Quaternion.identity);
        }
        else if(spawnBaseTile.gameObject.tag != "borderTile")
        {
            SetPlayerSpawn();
        }
        else if (spawnBaseTile.gameObject.tag != "emptyTile")
        {
            SetPlayerSpawn();
        }



       
    }

   /* void SpawnBlockCheck(int gridX, int gridY)
    {
        GameObject spawnBaseTile = tilePlacement2dArray[gridX, gridY + 1];
        if (spawnBaseTile.gameObject.tag == "solidTile")
        {
            Debug.Log("case of SOLID");
            spawnBaseTile = tilePlacement2dArray[gridX, gridY + 1];

            spawnBaseTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
            spawnBaseTile.gameObject.renderer.material = transparentMaterial;
            /*
            if (spawnBaseTile.gameObject.tag == "emptyTile")
            {
                GameObject playerSpawnInstance = (GameObject)Instantiate(playerSpawnPrefab, spawnBaseTile.transform.position, Quaternion.identity);
            }
            else if (spawnBaseTile.gameObject.tag != "emptyTile")
            {
                SetPlayerSpawn();
            }
            
        }
        else if (spawnBaseTile.gameObject.tag != "emptyTile")
        {
            Debug.Log("case of EMPTY");
            
        }
    }
    */
 
    void GetSurroundingWallCount(int gridX, int gridY)
    {
        //int wallCount = 0;
        GameObject checkedTile;
        checkedTile = tilePlacement2dArray[gridX, gridY];
        checkedTile.gameObject.renderer.material = standardBlockMaterial;
        if(checkedTile.gameObject.tag == "solidTile")
        {
            tileCheckerList.Clear();
            for (int neighbourX = gridX - 1;neighbourX <= gridX + 1; neighbourX++) //Breaking because it is using tiles in the exclusion list
            {
               for (int neighbourY = gridY - 1;neighbourY <= gridY + 1; neighbourY++)
               {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                    checkedTile = tilePlacement2dArray[neighbourX, neighbourY];
                        /*THESE BLOCK CHECKER RESULT VISUALISATION TOOLS, UNCOMMENT TO SEE THE PROCESS
                    checkedTile.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    checkedTile.gameObject.renderer.material = transparentMaterial;
                         */
                    tileCheckerList.Add(checkedTile);
                    //Debug.Log(checkedTile);
                    }

               }
            }        
        }
        //Debug.Log(tileCheckerList);
    }
}

    














/*
    public void CreateSomeGrid()
    {
        GameObject[,] tilePlacement2dArray = new GameObject[tileArraySizeX, tileArraySizeY]; 
        GameObject tileParent = new GameObject();
        tileParent.transform.position = spawnPoint.transform.position;
        tileParent.transform.rotation = spawnPoint.transform.rotation;
        var y = 0;

        for (int x = 0; x < tileArraySizeX; x++)
        {
            y++;
            GameObject tileInstance = (GameObject)Instantiate(tileObject, new Vector3(100, 100, 100), Quaternion.identity);
            tileInstance.name = ("tileName" + x);
            tileInstance.transform.parent = tileParent.transform;
            tileInstance.transform.position = new Vector3(
                                                    spawnPoint.position.x + x * -tileSize.x,
                                                    spawnPoint.position.y,
                                                    spawnPoint.position.z);
            tilePlacement2dArray[x, y] = tileInstance; 

        }

*/