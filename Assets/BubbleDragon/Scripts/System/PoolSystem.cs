using System.Collections;
using System.Collections.Generic;
using QMVC;
using UnityEngine;

public class PoolSystem : AbstractSystem
{
    MonoPool<BallController> ballPool;


    Transform poolRoot;



    AssetSystem _assetSystem;


    protected override void OnInit()
    {
        _assetSystem = this.GetSystem<AssetSystem>();

        poolRoot = new GameObject("Pools").transform;

        Transform ballParent = new GameObject(_assetSystem.ball.GetType().Name).transform;
        ballParent.SetParent(poolRoot);
        ballPool = new MonoPool<BallController>(_assetSystem.ball, ballParent);
    }



    public BallController GetBall()
    {
        return ballPool.Get();
    }

    public void RecycleBall(BallController ball)
    {
        ballPool.Recycle(ball);
    }
}
