using QMVC;

public class BaseController : IController
{
    public IArchitecture GetArchitecture()
    {
        return BubbleDragon.Interface;
    }
}
