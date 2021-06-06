using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{    
    static readonly int blockNum = 7;
    public GameObject[] blocks = new GameObject[blockNum];

    bool generateNextBlock = false; // 現在のブロックを落とし終えて、次のブロックを生成する際のトリガー

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        generateNextBlock = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(generateNextBlock)
        {
            int i = Random.Range(0, blockNum);
            GameObject nextBlock = Instantiate(blocks[i]) as GameObject;

            generateNextBlock = false;
        }
    }

    public bool GenerateNextBlock
    {
        set { generateNextBlock = value; }
        get { return generateNextBlock; }
    }
}
