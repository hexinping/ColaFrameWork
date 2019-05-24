using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHintTest : UIBase
{
    public UIHintTest(int resId, UILevel uiLevel) : base(resId, uiLevel)
    {
    }

    public UIHintTest()
    {

    }

    public override void Create()
    {
        base.Create();
        SetOutTouchDisappear();
    }

    protected override void onClick(string name)
    {
        if (name == "bg")
        {
            Debug.LogWarning("点击了bg按钮");
        }
    }
}
