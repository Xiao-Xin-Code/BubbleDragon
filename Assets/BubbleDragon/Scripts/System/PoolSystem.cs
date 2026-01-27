using QMVC;
using UnityEngine;

public class PoolSystem : AbstractSystem
{
    MonoPool<BallController> ballPool;
    ComponentPool<LineRenderer> linePool;

    Transform poolRoot;



    AssetSystem _assetSystem;


    protected override void OnInit()
    {
        _assetSystem = this.GetSystem<AssetSystem>();

        poolRoot = new GameObject("Pools").transform;

        Transform ballParent = new GameObject(_assetSystem.ball.GetType().Name).transform;
        ballParent.SetParent(poolRoot);
        ballPool = new MonoPool<BallController>(_assetSystem.ball, ballParent);
        Transform lineParent = new GameObject(_assetSystem.line.GetType().Name).transform;
        lineParent.SetParent(poolRoot);
        linePool = new ComponentPool<LineRenderer>(_assetSystem.line, lineParent);
    }



    public BallController GetBall()
    {
        return ballPool.Get();
    }

    public LineRenderer GetLine()
    {
        return linePool.Get();
    }

    public void RecycleBall(BallController ball)
    {
        ballPool.Recycle(ball);
    }

    public void RecycleLine(LineRenderer line)
    {
        linePool.Recycle(line);
    }



    public void RecycleAllLine()
    {
        linePool.RecycleAll();
    }


}
