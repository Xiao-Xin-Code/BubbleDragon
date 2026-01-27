
using UnityEngine.EventSystems;


public class BackGroundInputAllowEvent
{
	public bool isAllow;

	public BackGroundInputAllowEvent(bool isAllow)
	{
		this.isAllow = isAllow;
	}
}

public class BackGroundPointerDownEvent
{
	public PointerEventData eventData;

	public BackGroundPointerDownEvent(PointerEventData eventData)
	{
		this.eventData = eventData;
	}
}


public class BackGroundPointerUpEvent
{
	public PointerEventData eventData;

	public BackGroundPointerUpEvent(PointerEventData eventData)
	{
		this.eventData = eventData;
	}
}

public class BackGroundBeginDragEvent
{
	public PointerEventData eventData;

	public BackGroundBeginDragEvent(PointerEventData eventData)
	{
		this.eventData = eventData;
	}
}

public class BackGroundDragEvent
{
	public PointerEventData eventData;

	public BackGroundDragEvent(PointerEventData eventData)
	{
		this.eventData = eventData;
	}
}

public class BackGroundEndDragEvent
{
	public PointerEventData eventData;

	public BackGroundEndDragEvent(PointerEventData eventData)
	{
		this.eventData = eventData;
	}
}


public class PlaceToGridsEvent
{
	public BallController ball;

	public PlaceToGridsEvent(BallController ball)
	{
		this.ball = ball;
	}

}


public class EliminateBallEvent
{
	public BallController ball;

	public EliminateBallEvent(BallController ball)
	{
		this.ball = ball;
	}
}
