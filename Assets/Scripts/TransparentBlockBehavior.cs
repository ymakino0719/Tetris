using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentBlockBehavior : MonoBehaviour
{
    [SerializeField] GameObject blockManager;
    [SerializeField] GameObject currentBlock;
    BlockList bL;

    void Awake()
    {
        bL = blockManager.GetComponent<BlockList>();
    }
    // Update is called once per frame
    void Update()
    {
        if(CheckChildObjectExist()) // 子オブジェクトが存在するかどうかを確認
        {
            AdjustRotationAmountToCurrentBlock(); // CurrentBlockの回転量に合わせてTransparentBlockを回転させる
            DetermineExpectedFallPointOfCurrentBlock(); // CurrentBlockの落下予想地点を割り出し、TransparentBlockを移動する
        }
    }

    bool CheckChildObjectExist() // 子オブジェクトが存在するかどうかを確認
    {
        return (currentBlock.transform.childCount > 0 && transform.childCount > 0) ? true : false; // CurrentBlock、TransparentBlockそれぞれに子オブジェクトが存在しているとき、trueを返す
    }

    void AdjustRotationAmountToCurrentBlock() // CurrentBlockの回転量に合わせてTransparentBlockを回転させる
    {
        Vector3 rot = currentBlock.transform.GetChild(0).eulerAngles;
        transform.GetChild(0).eulerAngles = rot;
    }
    void DetermineExpectedFallPointOfCurrentBlock() // CurrentBlockの落下予想地点を割り出し、TransparentBlockを移動する
    {
        Transform currentChildTra = currentBlock.transform.GetChild(0);

        float downAmount = 0.0f; // currentBlockが降下できる量
        bool buttomPoint = false; // 確認箇所にブロックが存在するかどうか

        while (true)
        {
            foreach (Transform gao in currentChildTra)
            {
                Vector3 vec = gao.position;
                vec.y -= downAmount;

                if (Mathf.RoundToInt(vec.y) <= -1.0f || bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]) buttomPoint = true;// 確認箇所が最下層あるいはブロックが存在する場合、trueを返す
            }

            if (buttomPoint)
            {
                if(downAmount > 0.0f) downAmount -= 1.0f; // 最下層は１つ上のマスと判断し、１つ上に戻す（ただし、currentBlockが初期位置から降下できない場合は戻さない）
                break; // 確認箇所に１つでもブロックが存在している場合、ループを終了
            }
            else downAmount += 1.0f; // そうでない場合、１つ下にずらして確認を繰り返す
        }

        // TransparentBlockを落下予想地点に移動させる
        Vector3 point = currentChildTra.position;
        point.y -= downAmount;
        transform.GetChild(0).position = point;
    }
}
