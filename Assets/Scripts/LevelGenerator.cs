using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LevelGenerator : MonoBehaviour
{

    [Header("Game")]
    public GameObject LevelFinishScreen;

    public GameObject[] RaycastDownPositions; //3 by 3 grid raycast going down
    public GameObject[] RaycastDownOutsidePositions; //Raycasts outside of the grid

    public LayerMask PlacedCubesLayer;

    public bool check;

    public int minCubeCount;
    public int maxCubeCount;

    public GameObject[] allCubes;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel(); 
    }

    // Update is called once per frame
    void GenerateLevel()
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("pz_253_2457", typeof(TextAsset));
        string tileFile = txtAsset.text;


        string[] AllLines = tileFile.Split('\n');

        string[] pieces = AllLines[1].Split(',');

        Debug.Log("number of pieces" + (pieces.Length-1).ToString());

        for (int i = 0; i < pieces.Length-1; i++)
        {
            string[] text = AllLines[i*2+2].Split(',');
            int pieceNumber = int.Parse(text[0]);
            int numberOfBlocks = int.Parse(text[1]);


            string[] blockPositions = AllLines[i*2+3].Split(',');
            
            GameObject g = new GameObject();
            g.name = "Piece" + pieceNumber.ToString();
            g.transform.position = allCubes[int.Parse(blockPositions[0])].transform.position;

            Debug.Log("number of blocks" + (blockPositions.Length - 1).ToString());

            for (int a = 0; a < blockPositions.Length-1; a++)
            {
                int position = int.Parse(blockPositions[a]);
                Debug.Log(position);

                allCubes[position-1].transform.parent = g.transform; //-1 because of 0 indexing
            }

        }
    }

    string curString;
    void readTextFile(string file_path)
    {
        if (File.Exists(file_path))
        {
            StreamReader inp_stm = new StreamReader(file_path);

            while (!inp_stm.EndOfStream)
            {
                string text = inp_stm.ReadLine();
                curString = text;
            }

            inp_stm.Close();
        }
        else
        {
            Debug.Log("Not exist");
        }

    }

    private void Update()
    {
        if(check)
        {
            CheckGrid();
            check = false;
        }
    }



    //Level finish

    public void CheckGrid()
    {
        Invoke("CheckGridDelay", 0.5f);
       

    }
    void CheckGridDelay()
    {
        bool falseFlag = false;

        foreach (GameObject g in RaycastDownPositions)
        {
            RaycastHit[] hit;
            hit = Physics.RaycastAll(g.transform.position, -Vector3.up, 5f, PlacedCubesLayer);
            if(hit.Length != 3)
            {
                falseFlag = true;
            }
        }

        int outsideCubesHit = 0;

        foreach (GameObject g in RaycastDownOutsidePositions)
        {
            RaycastHit[] hit;
            hit = Physics.RaycastAll(g.transform.position, -Vector3.up, 5f, PlacedCubesLayer);
            outsideCubesHit += hit.Length;
        }

        if (falseFlag == false && outsideCubesHit == 0)
        {
            LevelFinishScreen.SetActive(true);
        }

    }
    public void Reset()
    {
        LevelFinishScreen.SetActive(false);
    }
}
