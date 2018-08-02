using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;
/* potatowang 2018-6-22
 * 
 */

/// <summary>
/// 调用parse函数进行解析，str为要解析的字符串。
/// </summary>
///  //富文本显示的类型

//被划分的item


public class UGUIRichParse : MonoBehaviour
{
    private delegate void AddTextItem(UGUIRichItem item,ref UGUILine mNewLine,int index);
    public UGUIRichStyle mRichStyle;
    private Regex mReEmoji;//正则表达式
    private Regex mReHref;
    private Regex mReUrl;
    private Regex mReColor;
    private SpriteAtlas mEmojiAtlas;
    private Queue<UGUIRichItem> mResultQueue;//解析结果队列
    private Queue<UGUILine> mResultLines;
    private List<string> mCutList;
    private float mCurWidth = 0;
    private float mMaxWidth = 0;
    private AddTextItem mAddTextItemHandler;
    
    #region Cache
    private MatchCollection mEmojiMatchCollection;
    private MatchCollection mHrefMatchCollection;
    #endregion


    //加载emoji表情的图片
   
    //初始化操作
    void Init()
    {
        mReEmoji = new Regex(@"\[.{1,3}\]");
        mReHref = new Regex(@"<a.+/a>");
        mReUrl = new Regex("href=#.*?#");
        mReColor = new Regex("color=#.*?#");
        mResultQueue = new Queue<UGUIRichItem>();
        mResultLines = new Queue<UGUILine>();
        mCutList = new List<string>();
        mEmojiAtlas = mRichStyle.EmojiAtlas;
        Debug.Log( HexToColor("0A0000FF"));
    }

    void Start()
    {
        Init();
        //parse("你在干[微笑]快来组队把什么[微笑]快来组队把<a href=#ww# color=#00FF00FF#>[组队邀请]</a>你在干什[微笑]快来组队把么[微笑]快来组队把<a href=#ww# color=#00FF00FF#>[组队邀请]</a>", 400);
    }
    
    string GetHrefInner(string href)
    {
        Regex re = new Regex(@">.*<");
        Match inner = re.Match(href);
        string result = inner.Value;
        result = result.Replace(">", "");
        result = result.Replace("<", "");
        return result;
    }

    public void CutStr(string str,float width,float mMaxWidth)
    {
        string forward = "";
        string back = "";
        float sum = 0;
        for(int i=0;i<str.Length;i++)
        {
            sum += mRichStyle.GetStrWidth(str[i].ToString());
            if(sum<=width)
            {
                forward += str[i];
            }else
            {
                back += str[i];
            }
        }
        mCutList.Add(forward);
        if (back == "")
            return;
        else
        {
            CutStr(back,mMaxWidth, mMaxWidth);
        }
    }
    void AddHrefToLine(UGUIRichItem item, ref UGUILine mNewLine,int index)
    {
        mNewLine.EnItemQueue(new UGUIRichItemHref(
            (item as UGUIRichItemHref).HrefNormalColor, mCutList[index], (item as UGUIRichItemHref).Url));
    }
    void AddTextToLine(UGUIRichItem item, ref UGUILine mNewLine,int index)
    {
        mNewLine.EnItemQueue(new UGUIRichItemText(
            Color.black, mCutList[index]));
    }
    void CreateTextItem(ref UGUIRichItem item,ref UGUILine mNewLine)
    {
        //如果没有溢出，则更新当前的宽度，向行中加入新元素
        if (mCurWidth + mRichStyle.GetStrWidth(item.Inner) <= mMaxWidth)
        {
            mCurWidth += mRichStyle.GetStrWidth(item.Inner);
            mNewLine.EnItemQueue(item);
        }
        //如果溢出，开始截断文字，溢出代表至少文字被分割成两个部分，如果被切割成三个部分，则第二部分一定是单独的一行
        else
        {
            mCutList.Clear();
            CutStr(item.Inner, mMaxWidth - mCurWidth, mMaxWidth);
            mAddTextItemHandler(item, ref mNewLine, 0);
            mResultLines.Enqueue(mNewLine);
            mCutList.RemoveAt(0);

            for (int i = 0; i < mCutList.Count; i++)
            {
                if (i != mCutList.Count - 1)
                {
                    UGUILine line = new UGUILine(0);
                    mAddTextItemHandler(item, ref line, i);
                    mResultLines.Enqueue(line);
                }
                else
                {
                    mCurWidth = mRichStyle.GetStrWidth(mCutList[i]);
                    mNewLine = new UGUILine(0);
                    mAddTextItemHandler(item, ref mNewLine, i);
                }
            }
        }
    }
    void CreateImageItem(ref UGUIRichItem item, ref UGUILine mNewLine)
    {
        UGUIRichItemImage imgItem = item as UGUIRichItemImage;
        if (mCurWidth + mRichStyle.EmojiSize.x <= mMaxWidth)
        {
            mCurWidth += mRichStyle.EmojiSpWidth;
            mNewLine.EnItemQueue(imgItem);
            mNewLine.LineNum = mRichStyle.EmojiHeightSpaceNum;
        }
        else
        {
            mResultLines.Enqueue(mNewLine);
            mNewLine = new UGUILine(0);
            mNewLine.EnItemQueue(imgItem);
            mNewLine.LineNum = mRichStyle.EmojiHeightSpaceNum;
            mCurWidth = mRichStyle.EmojiSize.x;
        }
    }

    //生成结果队列
    void GeneratorResult(string str)
    {
        string[] EmojiSplit = mReEmoji.Split(str);
        mEmojiMatchCollection = mReEmoji.Matches(str);
        for (int i = 0; i < EmojiSplit.Length; i++)
        {
            string[] HrefSplit = mReHref.Split(EmojiSplit[i]);
            mHrefMatchCollection = mReHref.Matches(EmojiSplit[i]);
            for (int j = 0; j < HrefSplit.Length; j++)
            {
                if (HrefSplit[j] != "")
                    mResultQueue.Enqueue(new UGUIRichItemText( 
                        Color.white,HrefSplit[j]));
                if (j != HrefSplit.Length - 1)
                {
                    string url = mReUrl.Match(mHrefMatchCollection[j].Value).Value.Replace("href=", "");
                    url = url.Replace("#", "");
                    string colorHex = mReColor.Match(mHrefMatchCollection[j].Value).Value.Replace("color=", "");
                    colorHex = colorHex.Replace("#", "");
                    Color hrefColor = HexToColor(colorHex);
                    mResultQueue.Enqueue(new UGUIRichItemHref(
                        hrefColor,GetHrefInner(mHrefMatchCollection[j].Value),url));
                }
            }
            if (i != EmojiSplit.Length - 1)
            {
                if (mRichStyle.isEmoji(mEmojiMatchCollection[i].Value))
                    mResultQueue.Enqueue(new UGUIRichItemImage(
                        Color.white, mEmojiMatchCollection[i].Value, mRichStyle.LoadSprite(mEmojiMatchCollection[i].Value),mRichStyle.EmojiSize));
                else
                    mResultQueue.Enqueue(new UGUIRichItemText(
                        Color.white,mEmojiMatchCollection[i].Value));
            }
        }
    }

    void GeneratorLines(float mMaxWidth)
    {
        UGUILine mNewLine = new UGUILine(0);
        mCurWidth = 0;
        while(mResultQueue.Count!=0)
        {
            UGUIRichItem item = mResultQueue.Dequeue();
            switch(item.ItemType)
            {
                case RichItemType.Text:
                    mAddTextItemHandler = AddTextToLine;
                    CreateTextItem(ref item,ref mNewLine);
                    break;
                case RichItemType.Image:
                    CreateImageItem(ref item, ref mNewLine);
                    break;
                case RichItemType.Href:
                    mAddTextItemHandler = AddHrefToLine;
                    CreateTextItem(ref item, ref mNewLine);
                    break;
            }
        }
        mResultLines.Enqueue(mNewLine);
    }

    public Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }

    //解析输入的字符串，将emoji占位符和文本区分开，并生成item
   

    public  Queue<UGUILine> parse(string str,float mWidth)
    {
        mMaxWidth = mWidth;
        GeneratorResult(str);
        GeneratorLines(mWidth);
        return mResultLines;
    }

}
