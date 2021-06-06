using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    static readonly int row = 10;
    static readonly int column = 25;

    bool[,] blocks = new bool[row, column];
    bool[] deleteLineBool = new bool[column];

    GameObject[] stackedLineList = new GameObject[column];

    // Start is called before the first frame update
    void Start()
    {
        for (int j = 0; j < column; j++)
        {
            for (int k = 0; k < row; k++)
            {
                blocks[k, j] = false;
            }
        }

        for (int i = 0; i < column; i++)
        {
            string stackedLineText;
            if (i <= 9) stackedLineText = "StackedLine0" + i;
            else stackedLineText = "StackedLine" + i;

            stackedLineList[i] = GameObject.Find(stackedLineText);
        }
    }

    public void CheckDeleteLines()
    {
        bool deleteBool = false;

        for (int j = 0; j < column; j++)
        {
            bool fullLine = true;

            for (int k = 0; k < row; k++)
            {
                if (!blocks[k, j]) fullLine = false;
            }

            if (fullLine)
            {
                deleteLineBool[j] = true;
                deleteBool = true;
                //Debug.Log("DeleteLine: " + j);
            } 
        }

        if(deleteBool) DeleteLine();
    }
    void DeleteLine()
    {
        for(int cNum = 0; cNum < column; cNum++)
        {
            if (deleteLineBool[cNum])
            {
                foreach (Transform n in stackedLineList[cNum].transform)
                {
                    //Debug.Log(n.gameObject);
                    Destroy(n.gameObject);
                }
            } 
        }

        DropLine();
    }
    void DropLine()
    {
        int deleteLineCount = 0;

        for (int cNum = 0; cNum < column; cNum++)
        {
            if (deleteLineBool[cNum])
            {
                deleteLineCount++;
                deleteLineBool[cNum] = false;
            }
            else
            {
                if (deleteLineCount >= 1)
                {
                    for (int rNum = 0; rNum < row; rNum++)
                    {
                        if (cNum + deleteLineCount < column)
                        {
                            blocks[rNum, cNum - deleteLineCount] = blocks[rNum, cNum];
                        }
                        else
                        {
                            blocks[rNum, cNum] = false;
                        }
                    }

                    if (cNum + deleteLineCount < column)
                    {
                        List<GameObject> bList = new List<GameObject>();
                        foreach (Transform n in stackedLineList[cNum].transform)
                        {
                            n.position += new Vector3(0, -deleteLineCount, 0);
                            bList.Add(n.gameObject);
                        }

                        foreach (GameObject gao in bList)
                        {
                            gao.transform.parent = stackedLineList[cNum - deleteLineCount].transform;
                        }
                    }
                }
            }
        }
    }


    public bool[,] Blocks
    {
        set { blocks = value; }
        get { return blocks; }
    }
    public GameObject[] StackedLineList
    {
        set { stackedLineList = value; }
        get { return stackedLineList; }
    }
}
