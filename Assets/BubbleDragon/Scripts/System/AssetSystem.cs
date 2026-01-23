using System.Collections;
using System.Collections.Generic;
using QMVC;
using UnityEngine;

public class AssetSystem : AbstractSystem
{
    public BallController ball { get; private set; }


    protected override void OnInit()
    {
		ball = Resources.Load<BallController>("Ball");
    }

}
