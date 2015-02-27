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
    private int amountToAdd = 25;
    private int addBlockCounter;

    //Tile Object dimensions
    public Vector3 tileSize;

    //gameObject to instantiate, I have it set to public so that it is changeable in the editor and I can test dimensional changes in various different object -- purely to debug
    public GameObject tileObject;
    //Player spawn instantiation WILL CHANGE THIS TO Resources.Load() folder eventually
    public GameObject playerSpawnPrefab;


    //Tile List
    private GameObject[,] tilePlacement2dArray;
    //Tile Settings List
    //private string[,] tileSettings2dArray;

    //Exclusion List
    public List<GameObject> tileExclusionList = new List<GameObject>();

    //Temp coordinate for tile/array
    private int coordX;
    private int coordY;


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
        //Debug.Log(coordX);
        //Debug.Log(coordY);
    }

    
    //Add all the border blocks to an exclusion list=====!
    //Get random array value
    //check against the exclusion values
    //if its valid, add it to exclusion list and then +1 to the completed counter
    //enable mesh and colider on block
    //iterate.

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
        //tileSelector.gameObject.renderer.material.color = Color.red;
        tileToSet.gameObject.GetComponent<MeshRenderer>().enabled = true;
        //Debug.Log(tileToSet);
        Debug.Log(addBlockCounter);
        addBlockCounter++;
    }


    void SetPlayerSpawn()
    {
        GetRandomTile();
        GameObject spawnSelector = tilePlacement2dArray[coordX, coordY];
        SpawnBlockCheck(spawnSelector);

        if(spawnSelector.gameObject.tag == "emptyTile")
        {
            GameObject playerSpawnInstance = (GameObject)Instantiate(playerSpawnPrefab, spawnSelector.transform.position, Quaternion.identity);
        }
        else if (spawnSelector.gameObject.tag != "emptyTile")
        {
            SetPlayerSpawn();
        }
    }

    void SpawnBlockCheck(GameObject currentBlock)
    {
    
    }
 
    void GetSurroundingWallCount(int gridX, int gridY)
    {
        //int wallCount = 0;
        GameObject checkedTile;
        checkedTile = tilePlacement2dArray[gridX, gridY];
        if(checkedTile.gameObject.tag == "solidTile")
        { 
            for (int neighbourX = gridX - 1;neighbourX <= gridX + 1; neighbourX++) //Breaking because it is using tiles in the exclusion list
            {
               for (int neighbourY = gridY - 1;neighbourY <= gridY + 1; neighbourY++)
               {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                    checkedTile = tilePlacement2dArray[neighbourX, neighbourY];     
                    Debug.Log(checkedTile);
                    }

               }
            }        
        }

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