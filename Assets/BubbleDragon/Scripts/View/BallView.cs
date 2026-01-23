using UnityEngine;

public class BallView : BaseView
{
    [SerializeField] SpriteRenderer iconRenderer;



    public void SetIconSprite(Sprite icon)
    {
        iconRenderer.sprite = icon;
    }
}
