using UnityEngine;

public class BallController : MonoController
{
    [SerializeField] BallView _ballView;
    BallEntity _ballEntity;

    public override void Init()
    {
        _ballEntity = new BallEntity();
    }




    public void UpdateIcon()
    {
        //_ballView.SetIconSprite();
    }
}
