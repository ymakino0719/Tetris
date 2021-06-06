using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    bool[,] blocks = new bool[10, 25];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool[,] Blocks
    {
        set { blocks = value; }
        get { return blocks; }
    }
}
