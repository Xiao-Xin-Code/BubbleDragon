using UnityEngine;

public class BallController : MonoController
{
    [SerializeField] BallView _ballView;
    BallEntity _ballEntity;


    public Vector2Int Coord { get => _ballEntity.coord; set => _ballEntity.coord = value; }


    public override void Init()
    {
        _ballEntity = new BallEntity();
    }




    public void UpdateIcon()
    {
        //_ballView.SetIconSprite();
    }
}
