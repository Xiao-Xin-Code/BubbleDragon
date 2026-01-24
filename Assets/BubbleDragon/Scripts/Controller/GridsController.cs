using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using QMVC;
using UnityEngine;

public class GridsController : MonoController
{
    PoolSystem _poolSystem;

    public override void Init()
    {
        _poolSystem = this.GetSystem<PoolSystem>();


        //CreateLevel();

	}


    public BallController ball;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            List<BallController> balls = new List<BallController>();
            CheckBall(balls, ball);
        }
    }



    public class BallCell
    {
        public BallController[] balls;
        public int count;
        public bool isShort;
	}


    Dictionary<int, BallCell> ballGrids = new Dictionary<int, BallCell>();

    int maxShowLine = -1;
    int minShowLine = -1;


    public void CreateLevel()
    {
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + "/Level/level_1.txt");
		float hspace = Mathf.Sqrt(3) / 2;

        float maxHeight = (lines.Length - 2) * hspace + 1;
        float showHeight = (11 - 1) * hspace + 1;

		for (int i = 1; i < lines.Length; i++)
        {
            float height = -(i - 1) * 1 * hspace - 0.5f;
			string[] contents = lines[i].Split(',');
			int halfCount = contents.Length / 2;
			float width = (contents.Length % 2 == 0) ? (halfCount * 1 - 0.5f) : halfCount * 1;
            BallCell ballCell = new BallCell();
            ballCell.balls = new BallController[contents.Length];
            ballCell.isShort = i % 2 != 0;
			for (int j = 0; j < contents.Length; j++)
            {
                if (contents[j] != "0")
                {
					BallController ball = _poolSystem.GetBall();
                    ball.transform.SetParent(transform);
                    ball.content = contents[j];
					ball.transform.localPosition = new Vector2(width - j * 1, height);
                    ball.Coord = new Vector2Int(i, j);
					ballCell.balls[j] = ball;
                    ballCell.count++;
				}
            }
            ballGrids.Add(i, ballCell);
        }

        float space = (maxHeight - showHeight) + 11;

        transform.DOMoveY(space, 2).SetEase(Ease.Linear);
	}


    public void CheckBall(List<BallController> results,BallController ball)
    {
        if (results.Contains(ball)) return;
		ball.GetComponent<SpriteRenderer>().color = Color.black;
		results.Add(ball);


        if (ballGrids[ball.Coord.x].isShort)
        {
            // 短行的邻居位置
            CheckNeighbor(results, ball, ball.Coord.x - 1, ball.Coord.y);        // 左上
			CheckNeighbor(results, ball, ball.Coord.x - 1, ball.Coord.y + 1);    // 右上
			CheckNeighbor(results, ball, ball.Coord.x, ball.Coord.y - 1);        // 左
			CheckNeighbor(results, ball, ball.Coord.x, ball.Coord.y + 1);        // 右
			CheckNeighbor(results, ball, ball.Coord.x + 1, ball.Coord.y);        // 左下
			CheckNeighbor(results, ball, ball.Coord.x + 1, ball.Coord.y + 1);    // 右下

		}
        else
        {
			// 长行的邻居位置
			CheckNeighbor(results, ball, ball.Coord.x - 1, ball.Coord.y - 1);    // 左上
			CheckNeighbor(results, ball, ball.Coord.x - 1, ball.Coord.y);        // 右上
			CheckNeighbor(results, ball, ball.Coord.x, ball.Coord.y - 1);        // 左
			CheckNeighbor(results, ball, ball.Coord.x, ball.Coord.y + 1);        // 右
			CheckNeighbor(results, ball, ball.Coord.x + 1, ball.Coord.y - 1);    // 左下
			CheckNeighbor(results, ball, ball.Coord.x + 1, ball.Coord.y);        // 右下
		}
	}



    private void CheckNeighbor(List<BallController> results,BallController currentBall,int neighborRow,int neighborCol)
    {
        if (!ballGrids.ContainsKey(neighborRow))
        {
            return;
        }

        var neighborRowData = ballGrids[neighborRow];
        if (neighborCol < 0 || neighborCol >= neighborRowData.balls.Length)
        {
            return;
        }

        BallController neighborBall = neighborRowData.balls[neighborCol];

        if (neighborBall != null && !results.Contains(neighborBall) && neighborBall.TypeEquals(currentBall))
        {
            CheckBall(results, neighborBall);
        }
    }



    public void Correction()
    {
        Vector3 end = Vector3.zero;

        //当碰撞到ball，基于ball判断需要的位置

        //如果到顶部时
        //先根据当前最高line，想办法获取到当前是否存在可显示的高层，
        //结合获取的高层的计算应该到达的位置

    }

}
