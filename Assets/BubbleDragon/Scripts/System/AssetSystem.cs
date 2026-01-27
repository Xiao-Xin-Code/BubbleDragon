using QMVC;
using UnityEngine;

public class AssetSystem : AbstractSystem
{
    public BallController ball { get; private set; }
    public LineRenderer line { get; private set; }

    protected override void OnInit()
    {
		ball = Resources.Load<BallController>("Ball");
        line = Resources.Load<LineRenderer>("Line");
    }

}
