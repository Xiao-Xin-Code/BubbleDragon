using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BallGridManager : MonoBehaviour
{
    public enum RowType
    {
        LongRow,
        ShortRow
    }

    public class BallData
    {
        public List<Ball> balls = new List<Ball>();
        public RowType rowType;
    }



    public struct BallStruct
    {
        public Ball[] balls;
        public RowType rowType;
    }

    public List<BallData> ballDatas = new List<BallData>();

    private Dictionary<int, BallStruct> ballGridDict = new Dictionary<int, BallStruct>();

    int curMaxRow = 0;

    public Ball prefab;
    public Vector2 startPos1;
    public Vector2 startPos2;

    public int longCount = 11;
    public int shortCount = 10;


    public List<Ball> result = new List<Ball>();


    void Start()
    {
        DOTween.Init();
        DOTween.SetTweensCapacity(1000, 300);

        float heightOffset = Mathf.Sqrt(3f) * 0.5f;
        for (int i = 0; i < 10; i++)
        {
            curMaxRow++;
            BallStruct ballStruct = new BallStruct();
            if (curMaxRow % 2 == 0)
            {
                ballStruct.rowType = RowType.LongRow;
                ballStruct.balls = new Ball[longCount];
                for(int j = 0; j < longCount; j++)
                {
                    Ball ball = Instantiate(prefab);
                    ball.val = 0;
                    ball.rowIndex = curMaxRow;
                    ball.colIndex = j;
                    ball.name = $"Ball[{ball.rowIndex}][{ball.colIndex}]";
                    ball.transform.position = startPos1 + new Vector2(j, -heightOffset * (9 - i));
                    ballStruct.balls[j] = ball;
                }
            }
            else
            {
                ballStruct.rowType = RowType.ShortRow;
                ballStruct.balls = new Ball[shortCount];
                for(int j = 0; j < shortCount; j++)
                {
                    Ball ball = Instantiate(prefab);
                    ball.val = 0;
                    ball.rowIndex = curMaxRow;
                    ball.colIndex = j;
                    ball.name = $"Ball[{ball.rowIndex}][{ball.colIndex}]";
                    ball.transform.position = startPos2 + new Vector2(j, -heightOffset * (9 - i));
                    ballStruct.balls[j] = ball;
                }
            }
            Debug.Log("添加数量" + ballStruct.balls.Length);
            ballGridDict.Add(curMaxRow, ballStruct);
        }
    }


    public List<Vector3> paths = new List<Vector3>();
    public Vector3 startPoint;
    public Vector3 dir;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExpandBalls();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            JudgePath(paths, startPoint, dir);
            prefab.transform.position = startPoint;
            prefab.transform.DOPath(paths.ToArray(), 5).OnComplete(PathComplete);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(result.Contains(ballGridDict[2].balls[3]));
        }
    }


    public void PathComplete()
    {
        CheckBall(result, prefab);

        Debug.Log(result.Contains(ballGridDict[2].balls[3]));
        if (result.Count >= 3)
        {
            for (int i = 0; i < result.Count; i++)
            {
                float randomAngle = UnityEngine.Random.Range(30, 150);
                Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.right;
                // 抛物线参数
                float jumpPower = UnityEngine.Random.Range(0.5f, 2f);    // 跳跃力度
                float duration = UnityEngine.Random.Range(0.5f, 1); // 动画时长
                int numJumps = 1;         // 跳跃次数
                Transform ballTransform = result[i].transform;

                Sequence sequence = DOTween.Sequence();
                sequence.Append(ballTransform.DOJump(result[i].transform.position + (Vector3)randomDirection * 1.5f, jumpPower, numJumps, duration)).SetEase(Ease.Linear);
                sequence.Insert(duration - 0.1f, ballTransform.DOMoveY(-15, 1.5f)).SetEase(Ease.Linear);
                sequence.OnComplete(() => Destroy(ballTransform.gameObject));
                sequence.SetAutoKill(true);
                sequence.Play();
            }
        }

        result.Clear();
    }


    private void ExpandBalls()
    {
        float heightOffset = Mathf.Sqrt(3f) * 0.5f;
        for(int i = 0; i < 3; i++)
        {
            curMaxRow++;
            BallStruct ballStruct = new BallStruct();

            if (curMaxRow % 2 == 0)
            {
                ballStruct.rowType = RowType.LongRow;
                ballStruct.balls = new Ball[longCount];
                for(int j = 0; j < longCount; j++)
                {
                    Ball ball = Instantiate(prefab);
                    ball.rowIndex = curMaxRow;
                    ball.colIndex = j;
                    ball.transform.position = startPos1 + new Vector2(j, -heightOffset * i);
                    ballStruct.balls[j] = ball;
                }
            }
            else
            {
                ballStruct.rowType = RowType.ShortRow;
                ballStruct.balls = new Ball[shortCount];
                for (int j = 0; j < shortCount; j++)
                {
                    Ball ball = Instantiate(prefab);
                    ball.rowIndex = curMaxRow;
                    ball.colIndex = j;
                    ball.transform.position = startPos2 + new Vector2(j, -heightOffset * i);
                    ballStruct.balls[j] = ball;
                }
            }
            ballGridDict.Add(curMaxRow, ballStruct);

        }
    }


    private void JudgePath(List<Vector3> path, Vector3 startPoint, Vector3 dir)
    {
        if (path.Count == 0)
        {
            path.Add(startPoint);
        }

        RaycastHit2D hit = Physics2D.CircleCast(startPoint, 0.5f, dir);
        if (hit.transform)
        {
            if (hit.transform.CompareTag("Top"))
            {
                float x = (int)hit.centroid.x;
                path.Add(new Vector3(x, hit.centroid.y, 0));

                Debug.Log($"目标位置{hit.centroid},调整位置{new Vector3(x, hit.centroid.y, 0)}");
                return;
                //hit.centroid;
            }
            else
            {
                if (hit.transform.CompareTag("Ball"))
                {
                    float heightOffset = Mathf.Sqrt(3f) * 0.5f;
                    var newDir = (Vector3)hit.point - hit.transform.position;
                    float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
                    if (angle < 0) angle += 360f;

                    if (angle > 330 && angle < 30)
                    {
                        path.Add(hit.transform.position + new Vector3(0.5f, 0));

                        //更新一圈的位置

                        RowType rowType = ballGridDict[hit.transform.GetComponent<Ball>().rowIndex].rowType;

                        prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex;
                        prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex + 1;

                        if (ballGridDict.ContainsKey(prefab.rowIndex))
                        {
                            ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                        }
                        else
                        {
                            BallStruct ballStruct = new BallStruct();
                            ballStruct.rowType = RowType.ShortRow;
                            ballStruct.balls = new Ball[shortCount];
                            ballStruct.balls[prefab.colIndex] = prefab;
                            ballGridDict.Add(prefab.rowIndex, ballStruct);
                        }

                    }
                    else if (angle > 30 && angle < 90)
                    {
                        path.Add(hit.transform.position + new Vector3(0.5f, heightOffset));

                        RowType rowType = ballGridDict[hit.transform.GetComponent<Ball>().rowIndex].rowType;

                        if (rowType == RowType.LongRow)
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex + 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.ShortRow;
                                ballStruct.balls = new Ball[shortCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }
                        else
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex + 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex - 1;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.LongRow;
                                ballStruct.balls = new Ball[longCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }

                    }
                    else if (angle > 90 && angle < 150)
                    {
                        path.Add(hit.transform.position + new Vector3(-0.5f, heightOffset));

                        RowType rowType = ballGridDict[hit.transform.GetComponent<Ball>().rowIndex].rowType;

                        if (rowType == RowType.LongRow)
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex + 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex - 1;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.ShortRow;
                                ballStruct.balls = new Ball[shortCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }
                        else
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex + 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.LongRow;
                                ballStruct.balls = new Ball[longCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }
                    }
                    else if (angle > 150 && angle < 210)
                    {
                        path.Add(hit.transform.position - new Vector3(0.5f, 0));


                        RowType rowType = ballGridDict[hit.transform.GetComponent<Ball>().rowIndex].rowType;

                        prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex;
                        prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex - 1;

                        if (ballGridDict.ContainsKey(prefab.rowIndex))
                        {
                            ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                        }
                        else
                        {
                            BallStruct ballStruct = new BallStruct();
                            ballStruct.rowType = RowType.ShortRow;
                            ballStruct.balls = new Ball[shortCount];
                            ballStruct.balls[prefab.colIndex] = prefab;
                            ballGridDict.Add(prefab.rowIndex, ballStruct);
                        }
                    }
                    else if (angle > 210 && angle < 270)
                    {
                        path.Add(hit.transform.position - new Vector3(0.5f, heightOffset));


                        RowType rowType = ballGridDict[hit.transform.GetComponent<Ball>().rowIndex].rowType;

                        if (rowType == RowType.LongRow)
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex - 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex - 1;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.ShortRow;
                                ballStruct.balls = new Ball[shortCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }
                        else
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex - 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.LongRow;
                                ballStruct.balls = new Ball[longCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }

                    }
                    else if (angle > 270 && angle < 330)
                    {
                        path.Add(hit.transform.position + new Vector3(0.5f, -heightOffset));

                        RowType rowType = ballGridDict[hit.transform.GetComponent<Ball>().rowIndex].rowType;

                        if(rowType == RowType.LongRow)
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex - 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.ShortRow;
                                ballStruct.balls = new Ball[shortCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }
                        else
                        {
                            prefab.rowIndex = hit.transform.GetComponent<Ball>().rowIndex - 1;
                            prefab.colIndex = hit.transform.GetComponent<Ball>().colIndex + 1;

                            if (ballGridDict.ContainsKey(prefab.rowIndex))
                            {
                                ballGridDict[prefab.rowIndex].balls[prefab.colIndex] = prefab;
                            }
                            else
                            {
                                BallStruct ballStruct = new BallStruct();
                                ballStruct.rowType = RowType.LongRow;
                                ballStruct.balls = new Ball[longCount];
                                ballStruct.balls[prefab.colIndex] = prefab;
                                ballGridDict.Add(prefab.rowIndex, ballStruct);
                            }
                        }
                    }

                    prefab.name = $"Ball[{prefab.rowIndex}][{prefab.colIndex}]";
                    return;
                }
                else
                {
                    path.Add(hit.centroid);
                }
            }

           
            var tempDir = Vector2.Reflect(dir, hit.normal);
            JudgePath(path, hit.centroid + tempDir.normalized * 0.01f, tempDir);
        }
    }


    private void CheckBall(List<Ball> results, Ball ball)
    {
        if (results.Contains(ball)) return;
        ball.GetComponent<SpriteRenderer>().color = Color.white;
        Debug.Log($"添加：[{ball.rowIndex}][{ball.colIndex}],改变颜色");
        results.Add(ball);
        if(ball.rowIndex ==2&&ball.colIndex == 3)
        {
            Debug.Log($"存在[2][3]{results.Contains(ball)}");
        }

        if(ballGridDict[ball.rowIndex].rowType == RowType.LongRow)
        {
            //是否存在上行
            // 长行的邻居位置
            CheckNeighbor(results, ball, ball.rowIndex - 1, ball.colIndex - 1);    // 左上
            CheckNeighbor(results, ball, ball.rowIndex - 1, ball.colIndex);        // 右上
            CheckNeighbor(results, ball, ball.rowIndex, ball.colIndex - 1);        // 左
            CheckNeighbor(results, ball, ball.rowIndex, ball.colIndex + 1);        // 右
            CheckNeighbor(results, ball, ball.rowIndex + 1, ball.colIndex - 1);    // 左下
            CheckNeighbor(results, ball, ball.rowIndex + 1, ball.colIndex);        // 右下
        }
        else
        {
            // 短行的邻居位置
            CheckNeighbor(results, ball, ball.rowIndex - 1, ball.colIndex);        // 左上
            CheckNeighbor(results, ball, ball.rowIndex - 1, ball.colIndex + 1);    // 右上
            CheckNeighbor(results, ball, ball.rowIndex, ball.colIndex - 1);        // 左
            CheckNeighbor(results, ball, ball.rowIndex, ball.colIndex + 1);        // 右
            CheckNeighbor(results, ball, ball.rowIndex + 1, ball.colIndex);        // 左下
            CheckNeighbor(results, ball, ball.rowIndex + 1, ball.colIndex + 1);    // 右下
        }
    }


    private void CheckNeighbor(List<Ball> results,Ball currentBall,int neighborRow,int neighborCol)
    {
        if (!ballGridDict.ContainsKey(neighborRow))
        {
            return;
        }

        var neighborRowData = ballGridDict[neighborRow];

        if (neighborCol < 0 || neighborCol >= neighborRowData.balls.Length)
        {
            return;
        }

        Ball neighborBall = neighborRowData.balls[neighborCol];

        // 检查邻居球是否存在、值是否相同、是否已经处理过
        if (neighborBall != null &&
            !results.Contains(neighborBall) &&
            neighborBall.val == currentBall.val)
        {
            CheckBall(results, neighborBall);
        }

    }
}
