using System.Collections;
using System.Collections.Generic;
using QMVC;
using UnityEngine;

public class BaseEntity : IEntity
{
    public IArchitecture GetArchitecture()
    {
        return BubbleDragon.Interface;
    }
}
