using QMVC;
using UnityEngine;

public abstract class MonoController : MonoBehaviour,IController
{
    private void Awake()
    {
        Init();
    }

    public abstract void Init();


    public IArchitecture GetArchitecture()
    {
        return BubbleDragon.Interface;
    }
}
