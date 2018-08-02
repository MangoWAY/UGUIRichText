using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UGUIRichGenerate : MonoBehaviour {
    public UGUIRichStyle RichStyle;
    public GameObjectPool RichItemPool;
    private Text mTextContent;
    private int mlineCount;
    private Queue<UGUILine> mResultLines;
    private Vector2 mCurPos = Vector2.zero;

    public void CreateEmojiPrefab(UGUIRichItem item)
    {
        var go = RichItemPool.GetGameObject(RichStyle.EmojiPrefabName);
        go.transform.SetParent(mTextContent.transform);
        go.GetComponent<RectTransform>().anchoredPosition = mCurPos;
        go.GetComponent<RectTransform>().sizeDelta = RichStyle.EmojiSize;
        go.GetComponent<Image>().sprite = (item as UGUIRichItemImage).ImageSprite;
    }
    public void CreateHrefPrefab(UGUIRichItem item)
    {
        var go = RichItemPool.GetGameObject(RichStyle.HrefPrefabName);
        go.transform.SetParent(mTextContent.transform);
        go.GetComponent<RectTransform>().anchoredPosition = mCurPos;
        go.GetComponent<Text>().text = item.Inner;
        go.GetComponent<Text>().color = (item as UGUIRichItemHref).HrefNormalColor;
    }
    public void generate(Queue<UGUILine> mResultLines,Text mTextContent)
    {
        this.mTextContent = mTextContent;
        mlineCount = mResultLines.Count;
        mCurPos = Vector2.zero;
        while (mResultLines.Count != 0)
        {
            mCurPos.x = 0;
            mCurPos.y -= RichStyle.LineHeight;
            UGUILine tmp = mResultLines.Dequeue();
            string m = "";
            for (int i = 0; i < tmp.LineNum; i++)
            {
                m += '\n';
                mCurPos.y -= RichStyle.LineHeight;
            }
            while (tmp.ItemCount != 0)
            {
                UGUIRichItem item = tmp.DeItemQueue();
                switch (item.ItemType)
                {
                    case RichItemType.Image:
                        m += RichStyle.EmojiWidthStr;
                        CreateEmojiPrefab(item);
                        mCurPos.x += RichStyle.EmojiSpWidth;
                        break;
                    case RichItemType.Text:
                        m += item.Inner;
                        mCurPos.x += RichStyle.GetStrWidth(item.Inner);
                        break;
                    case RichItemType.Href:
                        m += "<color=#FFFFFF00>" + item.Inner + "</color>";
                        CreateHrefPrefab(item);
                        mCurPos.x += RichStyle.GetStrWidth(item.Inner);
                        break;
                }
            }
            mTextContent.text += m;
            mTextContent.text += '\n';

        }
    }
}
