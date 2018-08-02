using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
public enum RichItemType
{
    Text,
    Image,
    Href,
    None
}

public class UGUILine
{
    private Queue<UGUIRichItem> ItemQueue;//每一行的元素
    public int LineNum = 0;
    public int ItemCount=0;
    public UGUILine(int lineNum)
    {
        ItemQueue = new Queue<UGUIRichItem>();
        LineNum = lineNum;
    }
    public void EnItemQueue(UGUIRichItem item)
    {
        ItemQueue.Enqueue(item);
        ItemCount++;
    }
    public UGUIRichItem DeItemQueue()
    {
        ItemCount--;
        return ItemQueue.Dequeue();
        
    }
    public void Reset()
    {
        LineNum = 0;
        ItemQueue.Clear();
        ItemCount = 0;
    }
}

public class UGUIRichItem{
    public RichItemType ItemType;
    public Color ItemColor;
    public string Inner;
    public UGUIRichItem(Color color,string inner)
    {
        ItemType = RichItemType.None;
        ItemColor = color;
        Inner = inner;
    }
}
public class UGUIRichItemImage:UGUIRichItem
{
    public Sprite ImageSprite;
    public Vector2 Size;
    public UGUIRichItemImage(Color color,string inner,Sprite sprite,Vector2 mSize):base(color,inner)
    {
        ImageSprite = sprite;
        Size = mSize;    
        ItemType = RichItemType.Image;
    }
}
public class UGUIRichItemHref : UGUIRichItem
{
    public string Url;
    public Color HrefNormalColor =Color.white;
    public Color HrefFoucsColor=Color.blue;
    public Color HrefClickColor=Color.yellow;
    public Color UnderlineColor=Color.red; 

    public UGUIRichItemHref(Color color, string inner, string url,Color hrefNormalColor, Color hrefFoucsColor, Color hrefClickColor, Color underlineColor) : base(color,inner)
    {
        Url = url;
        HrefNormalColor = hrefNormalColor;
        HrefFoucsColor = hrefFoucsColor;
        HrefClickColor = hrefClickColor;
        UnderlineColor = underlineColor;
        ItemType = RichItemType.Href;
    }
    public UGUIRichItemHref(Color color, string inner,string url):base(color,inner)
    {
        Url = url;
        ItemType = RichItemType.Href;
        HrefNormalColor = color;
    }

}
public class UGUIRichItemText : UGUIRichItem
{

    public UGUIRichItemText(Color color, string inner) : base(color,inner)
    {
        ItemType = RichItemType.Text;
    }

}

