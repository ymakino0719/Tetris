using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCurrentBlockMovement : MonoBehaviour
{
    [SerializeField] TransparentBlockBehavior tBB;

    // 現在操作中のブロックが動いたかどうか（生成された直後でもtrueを返す）
    bool moveCurrentBlock = false;

    void Update()
    {
        // CurrentBlock が動いた時の CurrentBlock 以外の処理
        if (moveCurrentBlock)
        {
            tBB.MoveTransparentBlockProcess();

            moveCurrentBlock = false;
        }
    }

    public bool MoveCurrentBlock
    {
        set { moveCurrentBlock = value; }
        get { return moveCurrentBlock; }
    }
}
