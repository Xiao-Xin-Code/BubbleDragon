using UnityEngine;

public class BallController : MonoController
{
    [SerializeField] BallView _ballView;
    BallEntity _ballEntity;


    public string content { get => _ballEntity.content; set => _ballEntity.content = value; }
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


public static class BallControllerExtension
{

    public static bool TypeEquals(this BallController self,BallController other)
    {
        return other != null && self.content == other.content;
    }

}
