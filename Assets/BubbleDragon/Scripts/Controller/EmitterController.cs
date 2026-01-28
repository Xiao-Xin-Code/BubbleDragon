using System.Collections.Generic;
using DG.Tweening;
using QMVC;
using UnityEngine;
using static GridsController;

public class EmitterController : MonoController
{
    PoolSystem _poolSystem;
	LevelModel _levelModel;


	RaycastHit2D[] hits = new RaycastHit2D[1];

	bool isCalculatingPath = false;
	public LayerMask layerMask;

	public override void Init()
    {
        _poolSystem = this.GetSystem<PoolSystem>();
		_levelModel = this.GetModel<LevelModel>();

        this.RegisterEvent<BackGroundDragEvent>(OnAim);
        this.RegisterEvent<BackGroundPointerUpEvent>(OnEmitter);
    }

	public List<Vector3> PathForecast(Vector3 startPoint, Vector3 dir, out RaycastHit2D resultHit)
    {
		List<Vector3> paths = new List<Vector3> { startPoint };
        int hitCount = Physics2D.CircleCastNonAlloc(startPoint, 0.5f, dir, hits, 50, layerMask);

        while (hitCount > 0 && hits[0].collider != null) 
        {
			Vector3 targetPoint;
			targetPoint = hits[0].centroid;
			paths.Add(targetPoint);

			if (hits[0].collider.CompareTag("Top Wall") || hits[0].collider.CompareTag("Ball"))
			{
				resultHit = hits[0];
				return paths;
			}

			if (hits[0].collider.CompareTag("Top Wall"))
            {
				float width = (11 % 2 == 0) ? -(5 * 1 - 0.5f) : -5 * 1;
				float offset = (targetPoint.x - width);
				int index = Mathf.RoundToInt(offset / 1);
				targetPoint.x = (width + index * 1);
			}
            if (hits[0].collider.CompareTag("Ball"))
            {
				resultHit = hits[0];
				float heightOffset = Mathf.Sqrt(3f) * 0.5f;
				var hitPoint = (Vector3)hits[0].point - hits[0].transform.position;
				float angle = Mathf.Atan2(hitPoint.y, hitPoint.x) * Mathf.Rad2Deg;
				if (angle < 0) angle += 360f;

				if (angle > 330 && angle < 30)
				{
					targetPoint = hits[0].transform.position + new Vector3(0.5f, 0);
				}
				else if (angle > 30 && angle < 90)
				{
					targetPoint = hits[0].transform.position + new Vector3(0.5f, heightOffset);
				}
				else if (angle > 90 && angle < 150)
				{
					targetPoint = hits[0].transform.position + new Vector3(-0.5f, heightOffset);
				}
				else if (angle > 150 && angle < 210)
				{
					targetPoint = hits[0].transform.position - new Vector3(0.5f, 0);
				}
				else if (angle > 210 && angle < 270)
				{
					targetPoint = hits[0].transform.position - new Vector3(0.5f, heightOffset);
				}
				else if (angle > 270 && angle < 330)
				{
					targetPoint = hits[0].transform.position + new Vector3(0.5f, -heightOffset);
				}
				return paths;
			}
			var newDir = Vector2.Reflect(dir, hits[0].normal);
            dir = newDir;
            Debug.DrawLine(targetPoint, targetPoint + (Vector3)newDir.normalized * 2, Color.yellow);
			hitCount = Physics2D.CircleCastNonAlloc(targetPoint + (Vector3)newDir.normalized * 0.1f, 0.5f, newDir.normalized, hits, 50, layerMask);
		}

		resultHit = new RaycastHit2D();
		return paths;
    }


    private void OnBeginAim(BackGroundPointerDownEvent evt)
    {

    }

    private void OnAim(BackGroundDragEvent evt)
    {
        if (!isCalculatingPath)
        {
			_poolSystem.RecycleAllLine();
            isCalculatingPath = true;
            if (evt.eventData.pointerCurrentRaycast.worldPosition.y > transform.position.y + 0.1f)
            {
                Vector3 dir = evt.eventData.pointerCurrentRaycast.worldPosition - transform.position;
                if (dir.normalized != Vector3.zero)
                {
                    try
                    {
						List<Vector3> paths = PathForecast(transform.position, dir,out RaycastHit2D hit);

						//更新路径
						//for(int i = 1; i < paths.Count; i++)
						//{
						//	LineRenderer line = _poolSystem.GetLine();
						//	line.positionCount = 2;
						//	line.SetPosition(0, paths[i - 1]);
						//	line.SetPosition(0, paths[i]);
						//}

                    }
                    finally
                    {
                        isCalculatingPath = false;
                    }
                }
            }
        }

    }

    private void OnEmitter(BackGroundPointerUpEvent evt)
    {
		//回收路径显示

		if (evt.eventData.pointerCurrentRaycast.worldPosition.y > transform.position.y + 0.1f)
		{
			Vector3 dir = evt.eventData.pointerCurrentRaycast.worldPosition - transform.position;
			if (dir.normalized != Vector3.zero)
			{
				List<Vector3> paths = PathForecast(transform.position, dir, out RaycastHit2D hit);

				//更新最后的实际位置
				Correction(paths, hit);

				//发射
				BallController ball = _poolSystem.GetBall();
				ball.transform.position = transform.position;
				//暂时屏蔽输入操作检测
				this.SendCommand(new BackGroundInputAllowCommand(false));

				ball.transform.DOPath(paths.ToArray(), 1).OnComplete(() =>
				{
					//先处理放置

					//触发消除处理
					
				});
			}
		}
		
	}


	private bool Correction(List<Vector3> paths, RaycastHit2D hit)
	{
		if(hit.collider.CompareTag("Top Wall"))
		{
			BallCell ballcell = this.SendCommand(new BallCellCommand(_levelModel.minRow));

			if (ballcell == null)
			{
				//直接按长行处理
				int halfcount = _levelModel.longRowCount / 2;
				float startPos = -(halfcount - 0.5f);
				float offset = hit.centroid.x - halfcount;
				int index = Mathf.RoundToInt(offset);
				paths[paths.Count - 1] = new Vector2(startPos + index, hit.centroid.y);
			}
			else
			{
				//按当前BallCell记录的长短使用
				if (ballcell.isShort)
				{
					int halfcount = _levelModel.shortRowCount / 2;
					float startPos = -halfcount;
					float offset = hit.centroid.x - halfcount;
					int index = Mathf.RoundToInt(offset);
					paths[paths.Count - 1] = new Vector2(startPos + index, hit.centroid.y);
				}
				else
				{
					int halfcount = _levelModel.longRowCount / 2;
					float startPos = -(halfcount - 0.5f);
					float offset = hit.centroid.x - halfcount;
					int index = Mathf.RoundToInt(offset);
					paths[paths.Count - 1] = new Vector2(startPos + index, hit.centroid.y);
				}
			}
		}
		else if (hit.collider.CompareTag("Ball"))
		{
			float heightOffset = Mathf.Sqrt(3f) * 0.5f;
			var dir = (Vector3)hit.point - hit.transform.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			//hit.transform.GetComponent<BallController>().Coord;

			Vector3 correctionPoint = Vector3.zero;
			if (angle < 0) angle += 360f;
			if (angle > 330 && angle < 30)
			{
				correctionPoint = hit.transform.position + new Vector3(0.5f, 0);
				Vector2 coord = hit.transform.GetComponent<BallController>().Coord + new Vector2(1, 0);
			}
			else if (angle > 30 && angle < 90)
			{
				correctionPoint = hits[0].transform.position + new Vector3(0.5f, heightOffset);

				Vector2 coord = hit.transform.GetComponent<BallController>().Coord;
			}
			else if (angle > 90 && angle < 150)
			{
				correctionPoint = hits[0].transform.position + new Vector3(-0.5f, heightOffset);
			}
			else if (angle > 150 && angle < 210)
			{
				correctionPoint = hits[0].transform.position - new Vector3(0.5f, 0);
			}
			else if (angle > 210 && angle < 270)
			{
				correctionPoint = hits[0].transform.position - new Vector3(0.5f, heightOffset);
			}
			else if (angle > 270 && angle < 330)
			{
				correctionPoint = hits[0].transform.position + new Vector3(0.5f, -heightOffset);
			}
			paths[paths.Count - 1] = correctionPoint;

			

		}

		//获取新坐标
		return false;
	}

}
