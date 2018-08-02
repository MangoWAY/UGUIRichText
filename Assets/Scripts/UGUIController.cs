using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUIController : MonoBehaviour {
    public UGUIRichParse mParse;
    public UGUIRichStyle mStyle;
    public UGUIRichGenerate mGenerate;
    public Text mText;
    public float Width;

    public void Display(string str)
    {
        mStyle.Init(mText);
        mGenerate.generate(mParse.parse(str, Width),mText);
    }
}
