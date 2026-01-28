using QMVC;
using UnityEngine.EventSystems;
using static GridsController;


public class BackGroundInputAllowCommand : AbstractCommand
{
	bool isAllow;

	public BackGroundInputAllowCommand(bool isAllow)
	{
		this.isAllow = isAllow;
	}

    protected override void OnExecute()
    {
		this.SendEvent(new BackGroundInputAllowEvent(isAllow));
    }
}

public class BackGroundPointerDownCommand : AbstractCommand
{
    PointerEventData eventData;

    public BackGroundPointerDownCommand(PointerEventData eventData)
    {
        this.eventData = eventData;
    }

    protected override void OnExecute()
    {
        this.SendEvent(new BackGroundPointerDownEvent(eventData));
    }
}

public class BackGroundPointerUpCommand : AbstractCommand
{
	PointerEventData eventData;

	public BackGroundPointerUpCommand(PointerEventData eventData)
	{
		this.eventData = eventData;
	}


	protected override void OnExecute()
    {
		this.SendEvent(new BackGroundPointerUpEvent(eventData));
    }
}

public class BackGroundBeginDragCommand : AbstractCommand
{
	PointerEventData eventData;

	public BackGroundBeginDragCommand(PointerEventData eventData)
	{
		this.eventData = eventData;
	}

	protected override void OnExecute()
    {
		this.SendEvent(new BackGroundBeginDragEvent(eventData));
    }
}

public class BackGroundDragCommand : AbstractCommand
{
	PointerEventData eventData;

	public BackGroundDragCommand(PointerEventData eventData)
	{
		this.eventData = eventData;
	}

	protected override void OnExecute()
    {
		this.SendEvent(new BackGroundDragEvent(eventData));
    }
}

public class BackGroundEndDragCommand : AbstractCommand
{
	PointerEventData eventData;

	public BackGroundEndDragCommand(PointerEventData eventData)
	{
		this.eventData = eventData;
	}

	protected override void OnExecute()
    {
		this.SendEvent(new BackGroundEndDragEvent(eventData));
    }
}


public class PlaceToGridsCommand : AbstractCommand
{
	BallController ball;

	public PlaceToGridsCommand(BallController ball)
	{
		this.ball = ball;
	}


    protected override void OnExecute()
    {
		this.SendEvent(new PlaceToGridsEvent(ball));
    }
}


public class EliminateBallCommand : AbstractCommand
{
	BallController ball;

	public EliminateBallCommand(BallController ball)
	{
		this.ball = ball;
	}

    protected override void OnExecute()
    {
		this.SendEvent(new EliminateBallEvent(ball));
    }
}


public class BallCellCommand : AbstractCommand<BallCell>
{
	int row;

	public BallCellCommand(int row)
	{
		this.row = row;
	}

    protected override BallCell OnExecute()
    {
		BallCellEvent evt = new BallCellEvent(row);
		this.SendEvent(evt);
		return evt.ballcell;
	}
}