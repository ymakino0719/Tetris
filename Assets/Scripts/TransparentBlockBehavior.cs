using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentBlockBehavior : MonoBehaviour
{
    [SerializeField] GameObject currentBlock;
    [SerializeField] BlockList bL;

    // Update is called once per frame
    void Update()
    {
        if (CheckChildObjectExist())
        {
            // CurrentBlockの回転量に合わせてTransparentBlockを回転させる
            AdjustRotationAmountToCurrentBlock();
            // CurrentBlockの落下予想地点を割り出し、TransparentBlockを移動する
            DetermineExpectedFallPointOfCurrentBlock();
        }
    }

    // 子オブジェクトが存在するかどうかを確認
    bool CheckChildObjectExist()
    {
        // CurrentBlock、TransparentBlockそれぞれに子オブジェクトが存在しているとき、trueを返す
        return (currentBlock.transform.childCount > 0 && transform.childCount > 0) ? true : false;
    }

    // CurrentBlockの回転量に合わせてTransparentBlockを回転させる
    void AdjustRotationAmountToCurrentBlock()
    {
        Vector3 rot = currentBlock.transform.GetChild(0).eulerAngles;
        transform.GetChild(0).eulerAngles = rot;
    }

    // CurrentBlockの落下予想地点を割り出し、TransparentBlockを移動する
    void DetermineExpectedFallPointOfCurrentBlock()
    {
        Transform currentChildTra = currentBlock.transform.GetChild(0);

        // currentBlockが降下できる量
        float downAmount = 0.0f;
        // 確認箇所にブロックが存在するかどうか
        bool buttomPoint = false;

        while (true)
        {
            foreach (Transform gao in currentChildTra)
            {
                Vector3 vec = gao.position;
                vec.y -= downAmount;

                // 確認箇所が最下層あるいはブロックが存在する場合、trueを返す
                if (Mathf.RoundToInt(vec.y) <= -1.0f || bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]) buttomPoint = true;
            }

            if (buttomPoint)
            {
                // 最下層は１つ上のマスと判断し、１つ上に戻す（ただし、currentBlockが初期位置から降下できない場合は戻さない）
                if (downAmount > 0.0f) downAmount -= 1.0f;
                // 確認箇所に１つでもブロックが存在している場合、ループを終了
                break;
            }
            else
            {
                // そうでない場合、１つ下にずらして確認を繰り返す
                downAmount += 1.0f;
            }
        }

        // TransparentBlockを落下予想地点に移動させる
        Vector3 point = currentChildTra.position;
        point.y -= downAmount;
        transform.GetChild(0).position = point;
    }
}
