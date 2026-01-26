using QMVC;
using UnityEngine.EventSystems;


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