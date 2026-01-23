using QMVC;
using UnityEngine;

public class BaseView : MonoBehaviour, IView
{
    public IArchitecture GetArchitecture()
    {
        return BubbleDragon.Interface;
    }
}
