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


        CreateLevel();

	}


    public class BallCell
    {
        BallController[] balls;
        int count;
	}


    Dictionary<int, BallController[]> ballGrids = new Dictionary<int, BallController[]>();

    public void CreateLevel()
    {
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + "/Level/level_1.txt");
		float hspace = Mathf.Sqrt(3) / 2;

        float maxHeight = (lines.Length - 1) * hspace + 1;
        float showHeight = (11 - 1) * hspace + 1;

		for (int i = 1; i < lines.Length; i++)
        {
            float height = -(i - 1) * 1 * hspace - 0.5f;
			string[] contents = lines[i].Split(',');
			int halfCount = contents.Length / 2;
			float width = (contents.Length % 2 == 0) ? (halfCount * 1 - 0.5f) : halfCount * 1;
            BallController[] balls = new BallController[contents.Length];
			for (int j = 0; j < contents.Length; j++)
            {
                if (contents[j] != "0")
                {
					BallController ball = _poolSystem.GetBall();
                    ball.transform.SetParent(transform);
					ball.transform.localPosition = new Vector2(width - j * 1, height);
                    ball.Coord = new Vector2Int(i, j);
                    balls[j] = ball;
				}
            }
            ballGrids.Add(i, balls);
        }

        float space = (maxHeight - showHeight) + 11;

        transform.DOMoveY(space, 1).SetEase(Ease.Linear);
	}


    public void CheckBall(List<BallController> results,BallController ball)
    {



    }

}
