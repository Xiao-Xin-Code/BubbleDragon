using System.Collections.Generic;
using DG.Tweening;
using QMVC;
using UnityEngine;

public class EmitterController : MonoController
{
    PoolSystem _poolSystem;

    public override void Init()
    {
        _poolSystem = this.GetSystem<PoolSystem>();
    }
    public List<Vector3> paths = new List<Vector3>();


	private void Update()
    {

        if (paths.Count > 0) 
        {
            Debug.DrawLine(transform.position, paths[0], Color.red);
		}
		for (int i = 1; i < paths.Count; i++)
		{
			Debug.DrawLine(paths[i - 1], paths[i], Color.red);
		}
        if(Input.GetMouseButton(0)&& !isCalculatingPath)
		{
            isCalculatingPath = true;
			Vector3 clickWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickWorldPos.z = 0;
            if (clickWorldPos.y > transform.position.y + 0.1f) 
            {
				//Debug.Log(clickWorldPos);
                Vector3 dir = clickWorldPos - transform.position;
                if (dir.normalized != Vector3.zero)
                {
                    Debug.Log("º∆À„");
                    try
                    {
						paths.Clear();
						PathForecast(paths, transform.position, dir);
					}
                    finally
                    {
                        isCalculatingPath = false;

					}
				
				}
			}
            else
            {
                isCalculatingPath = false;
            }
		}

        if (Input.GetMouseButtonUp(0))
        {
            //Emitter();
		}
        

	}
    public bool isCalculatingPath = false;
	RaycastHit2D[] hits = new RaycastHit2D[1];
    public LayerMask layerMask;


    private void Emitter()
    {
        BallController ball = _poolSystem.GetBall();
        ball.transform.DOPath(paths.ToArray(),1);
    }


    private void OnDrawGizmos()
    {
        if (isHas)
        {
            Gizmos.DrawWireSphere(end, 0.5f);
		}
    }

    bool isHas = false;
    Vector3 end;

    public void PathForecast(List<Vector3> paths, Vector3 startPoint, Vector3 dir)
    {
        int hitCount = Physics2D.CircleCastNonAlloc(startPoint, 0.5f, dir, hits, 50, layerMask);
        paths.Add(startPoint);
        while (hitCount > 0 && hits[0].collider != null) 
        {
			Vector3 targetPoint;

			targetPoint = hits[0].centroid;
            paths.Add(targetPoint);

            if (hits[0].transform.name == "Top")
            {
				float width = (11 % 2 == 0) ? -(5 * 1 - 0.5f) : -5 * 1;

                float offset = (targetPoint.x - width);
                int index = Mathf.RoundToInt(offset / 1);

                targetPoint.x = (width + index * 1);
                end = targetPoint;
                isHas = true;
				return;
            }

            if(hits[0].transform.name == "Ball(Clone)")
            {
				float heightOffset = Mathf.Sqrt(3f) * 0.5f;
				var hitPoint = (Vector3)hits[0].point - hits[0].transform.position;
				float angle = Mathf.Atan2(hitPoint.y, hitPoint.x) * Mathf.Rad2Deg;
				if (angle < 0) angle += 360f;

				if (angle > 330 && angle < 30)
                {
                    end = hits[0].transform.position + new Vector3(0.5f, 0);
				}
				else if (angle > 30 && angle < 90)
                {
                    end = hits[0].transform.position + new Vector3(0.5f, heightOffset);
				}
				else if (angle > 90 && angle < 150)
                {
                    end = hits[0].transform.position + new Vector3(-0.5f, heightOffset);
				}
				else if (angle > 150 && angle < 210)
                {
                    end = hits[0].transform.position - new Vector3(0.5f, 0);
				}
				else if (angle > 210 && angle < 270)
                {
                    end = hits[0].transform.position - new Vector3(0.5f, heightOffset);
				}
				else if (angle > 270 && angle < 330)
                {
                    end = hits[0].transform.position + new Vector3(0.5f, -heightOffset);
				}

                isHas = true;
				return;
			}

            
			var newDir = Vector2.Reflect(dir, hits[0].normal);
            dir = newDir;
            Debug.DrawLine(targetPoint, targetPoint + (Vector3)newDir.normalized * 2, Color.yellow);
			hitCount = Physics2D.CircleCastNonAlloc(targetPoint + (Vector3)newDir.normalized * 0.1f, 0.5f, newDir.normalized, hits, 50, layerMask);
            Debug.Log(hitCount);
		}
    }
}
