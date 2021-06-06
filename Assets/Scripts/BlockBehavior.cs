using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    BlockList bL;

    List<GameObject> currentBlockList = new List<GameObject>();

    float waitTime = 1.0f;

    float longPressTime_Hor = 0.0f;
    float longPressTime_Ver = 0.0f;
    float timeCoef_Hor = 0.0f;
    float timeCoef_Ver = 1.0f;
    float fastMoveCoef = 15.0f;
    float maxPressTime = 0.8f;

    Vector3 rightDir = new Vector3(1.0f, 0, 0);
    Vector3 leftDir = new Vector3(-1.0f, 0, 0);
    Vector3 downDir = new Vector3(0, -1.0f, 0);

    void Awake()
    {
        bL = GameObject.Find("BlockList").GetComponent<BlockList>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in this.gameObject.transform) currentBlockList.Add(child.gameObject);
        StartCoroutine("FallBlockCoroutine");
    }

    // Update is called once per frame
    void Update()
    {
        ShortPressMovement();
        LongPressMovement();
    }

    private IEnumerator FallBlockCoroutine()
    {
        yield return new WaitForSeconds(waitTime);

        while(Input.GetButton("DownKey")) yield return null;

        SlideBlock(downDir, true);
        StartCoroutine("FallBlockCoroutine");
    }

    void StackAndGenerateBlocks()
    {
        GameObject stack = GameObject.Find("StackedBlocks");

        for (int j = 0; j < currentBlockList.Count; j++)
        {
            Vector3 vec = currentBlockList[j].transform.position;
            bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)] = true;
            currentBlockList[j].transform.parent = stack.transform;
        } 

        GameObject.Find("BlockGenerator").GetComponent<BlockGenerator>().GenerateNextBlock = true;

        Destroy(this.gameObject);
    }

    void ShortPressMovement()
    {
        if (Input.GetButtonDown("RightKey"))
        {
            SlideBlock(rightDir, false);
        }

        if (Input.GetButtonDown("LeftKey"))
        {
            SlideBlock(leftDir, false);
        }

        if (Input.GetButtonDown("DownKey"))
        {
            SlideBlock(downDir, true);
        }

        if (Input.GetButtonDown("UpKey"))
        {
            RotateBlock();
        }
    }
    void SlideBlock(Vector3 dir, bool down)
    {
        foreach (GameObject gao in currentBlockList)
        {
            Vector3 vec = gao.transform.position;
            vec += dir;

            if (vec.x <= -1.0f || vec.x >= 10.0f) return;

            if (vec.y <= -1.0f)
            {
                StackAndGenerateBlocks();
                return;
            } 

            if (bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)])
            {
                if (down) StackAndGenerateBlocks();
                return;
            }
        }

        this.gameObject.transform.Translate(dir, Space.World);
    }

    void RotateBlock()
    {
        bool normalRotate = false, rightRotate = false, leftRotate = false;

        normalRotate = RotatePossibility(Vector3.zero);
        if (!normalRotate) rightRotate = RotatePossibility(rightDir);
        if (!normalRotate && !rightRotate) leftRotate = RotatePossibility(leftDir);

        Debug.Log("normalRotate = " + normalRotate + ", rightRotate = " + rightRotate + ", leftRotate = " + leftRotate);

        if (!normalRotate && !rightRotate && !leftRotate) return;
        else if(rightRotate) transform.position += rightDir;
        else if(leftRotate) transform.position += leftDir;

        transform.Rotate(0, 0, 90.0f);
    }

    bool RotatePossibility(Vector3 dir)
    {
        foreach (GameObject gao in currentBlockList)
        {
            Quaternion rot = Quaternion.Euler(0, 0, 90.0f);
            Vector3 point = gao.transform.position + dir;
            Vector3 center = transform.position + dir;
            Vector3 vec = rot * (point - center) + center;
            Vector2 vecInt = new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));

            if (vecInt.x <= -1.0f || vecInt.x >= 10.0f || vecInt.y <= -1.0f) return false;
            else if(bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]) return false;
        }

        return true;
    }

    void LongPressProcess(Vector3 dir, ref float longPressTime, ref float timeCoef, bool down)
    {
        longPressTime += timeCoef * Time.deltaTime;

        if (longPressTime >= maxPressTime)
        {
            SlideBlock(dir, down);

            longPressTime = 0.0f;
            timeCoef = fastMoveCoef;
        }
    }

    void LongPressMovement()
    {
        if (Input.GetButton("RightKey") && !Input.GetButton("LeftKey")) // 右矢印だけ押されているとき
        {
            LongPressProcess(rightDir, ref longPressTime_Hor, ref timeCoef_Hor, false);
        }
        else if (!Input.GetButton("RightKey") && Input.GetButton("LeftKey")) // 左矢印だけ押されているとき
        {
            LongPressProcess(leftDir, ref longPressTime_Hor, ref timeCoef_Hor, false);
        }
        else // それ以外の時（両方同時に押されているか、両方押されていない場合）
        {
            longPressTime_Hor = 0.0f;
            timeCoef_Hor = 1.0f;
        }

        if (Input.GetButton("DownKey"))
        {
            LongPressProcess(downDir, ref longPressTime_Ver, ref timeCoef_Ver, true);
        }
        else
        {
            longPressTime_Ver = 0.0f;
            timeCoef_Ver = 1.0f;
        }
    }
}
