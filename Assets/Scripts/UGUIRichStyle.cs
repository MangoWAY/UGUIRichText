using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
public class UGUIRichStyle : MonoBehaviour {

    #region EmojiSettings
    public GameObject EmojiPrefab;
    public GameObject HrefPrefab;
    public SpriteAtlas EmojiAtlas;
    public Vector2 EmojiSize=new Vector2(40,40);
    public string EmojiPrefabName;
    public string HrefPrefabName;
    [HideInInspector] public string EmojiWidthStr="";//空格字符串来代替emoji占位
    [HideInInspector] public string EmojiHeightStr ="";
    #endregion

    #region

    [HideInInspector] public float LineHeight;//每行的高度
    [HideInInspector] public float SpaceWidth;//空格的宽度
    [HideInInspector] public int EmojiWidthSpaceNum = 0;//一个emoji需要的空格
    [HideInInspector] public int EmojiHeightSpaceNum = 0;//一个emoji需要的占的行数
    [HideInInspector] public float EmojiSpWidth = 0;//emoji空格的宽度
    private float mPixelsPerUnit;
    private TextGenerator mTextGenerator;
    private TextGenerationSettings mTgSettings;
    #endregion

    private void Start()
    {
    }


    public void Init(Text TextTemplate)
    {
        mTextGenerator = TextTemplate.cachedTextGeneratorForLayout;
        mTgSettings = TextTemplate.GetGenerationSettings(Vector2.zero);
        mPixelsPerUnit = TextTemplate.pixelsPerUnit;

        HrefPrefab.GetComponent<Text>().fontSize = TextTemplate.fontSize;

        LineHeight = mTextGenerator.GetPreferredHeight("好", mTgSettings) /mPixelsPerUnit;

        SpaceWidth = GetStrWidth(" ");

        EmojiWidthSpaceNum = (EmojiSize.x / SpaceWidth) % 1 < 0.05f ? (int)(EmojiSize.x / SpaceWidth) : (int)(EmojiSize.x / SpaceWidth) + 1;
        EmojiHeightSpaceNum = (EmojiSize.y / LineHeight) % 1 < 0.05f ? (int)(EmojiSize.y / LineHeight) -1 : (int)(EmojiSize.y / LineHeight) ;
        EmojiWidthStr = "";
        EmojiHeightStr = "";
        for(int i=0;i< EmojiWidthSpaceNum; i++)
        {
            EmojiWidthStr += " ";
        }
        for (int i = 0; i < EmojiHeightSpaceNum; i++)
        {
            EmojiHeightStr += '\n';
        }
        EmojiSpWidth = GetStrWidth(EmojiWidthStr);
        
    }
 
    public float GetStrWidth(string str)
    {
        return mTextGenerator.GetPreferredWidth(str, mTgSettings) / mPixelsPerUnit;
    }

    public  Sprite LoadSprite(string spriteName)
    {
        return EmojiAtlas.GetSprite(spriteName);
    }
    public bool isEmoji(string str)
    {
        try { LoadSprite(str); return true; }
        catch { return false; }
    }

}
