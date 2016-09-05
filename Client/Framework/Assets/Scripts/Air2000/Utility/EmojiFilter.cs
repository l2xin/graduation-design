using UnityEngine;
using System.Collections;

public class EmojiFilter{

    private const int FilterEmoji_MaxLen = 1024;  //如果文字过长就截断

    public static string FilterEmoji(string str)
    {
        return FilterEmoji(System.Text.Encoding.UTF8.GetBytes(str));

    }
    public static string FilterEmoji(byte[] bStr)
    {
        if (bStr == null)
        {
            return "";
        }

        int length = bStr.Length;
		if(length > FilterEmoji_MaxLen)//如果文字过长就截断
			length=FilterEmoji_MaxLen;

        byte[] target = new byte[FilterEmoji_MaxLen];
        
        int targetId=0;
        byte[] source = bStr;
        
        for(int i = 0; i < length; i++)
        {
            byte tempChar = source[i];
            
            if(tempChar >= 0 && tempChar < 0xc0)
            {
                target[targetId++] = source[i];
            }else if(tempChar >= 0xc0 && tempChar < 0xe0)
            {
                int unicode = DecodeUtfCodeTwoChar(source[i],source[i+1]);
                if(IsEmoji(unicode))
                {
                    i += 1;
                    target[targetId++] = 0x20;
                }else
                {
                    target[targetId++] = source[i];
                }
            }else if(tempChar >= 0xe0 && tempChar < 0xf0)
            {
                int unicode = (i+2 <= length) ? DecodeUtfCodeThreeChar(source[i],source[i+1],source[i+2]) : 0;
                if(IsEmoji(unicode))
                {
                    i += 2;
                    target[targetId++] = 0x20;
                }else
                {
                    target[targetId++] = source[i];
                }
            }else if(tempChar >= 0xf0 && tempChar < 0xf8)
            {
                int unicode = (i+3 <= length) ? DecodeUtfCodeFourChar(source[i],source[i+1],source[i+2],source[i+3]) : 0;
                if(IsEmoji(unicode))
                {
                    i += 3;
                    target[targetId++] = 0x20;
                }else
                {
                    target[targetId++] = source[i];
                }
            }else if(tempChar >= 0xf8 && tempChar < 0xfc)
            {
                int unicode = (i+4 <= length) ? DecodeUtfCodeFiveChar(source[i],source[i+1],source[i+2],source[i+3],source[i+4]) : 0;
                if(IsEmoji(unicode))
                {
                    i += 4;
                    target[targetId++] = 0x20;
                }else
                {
                    target[targetId++] = source[i];
                }
            }else if(tempChar >= 0xfc && tempChar <= 0xff)
            {
                int unicode = (i+5 <= length) ? DecodeUtfCodeSixChar(source[i],source[i+1],source[i+2],source[i+3],source[i+4],source[i+5]) : 0;
                if(IsEmoji(unicode))
                {
                    i += 5;
                    target[targetId++] = 0x20;
                }else
                {
                    target[targetId++] = source[i];
                }
            }
        }
        return System.Text.Encoding.UTF8.GetString(target, 0, targetId);
    }

    private static int DecodeUtfCodeTwoChar(byte a, byte b)
    {
        int ret = 0;
        int bb = (b&0x3f);
        int aa = (a&0x1f)<<6;
        ret = bb|aa;
        return ret;
    }
    
    private static int DecodeUtfCodeThreeChar(byte a, byte b, byte c)
    {
        int ret = 0;
        int cc = (c&0x3f);
        int bb = (b&0x3f)<<6;
        int aa = (a&0xf)<<12;
        ret = cc|bb|aa;
        return ret;
    }
    
    private static int DecodeUtfCodeFourChar(byte a, byte b, byte c , byte d)
    {
        int ret = 0;
        int dd = (d&0x3f);
        int cc = (c&0x3f)<<6;
        int bb = (b&0x3f)<<12;
        int aa = (a&0x7)<<18;
        ret = dd|cc|bb|aa;
        return ret;
    }
    
    private static int DecodeUtfCodeFiveChar(byte a, byte b, byte c , byte d, byte e)
    {
        int ret = 0;
        int ee = (e&0x3f);
        int dd = (d&0x3f)<<6;
        int cc = (c&0x3f)<<12;
        int bb = (b&0x3f)<<18;
        int aa = (a&0x3)<<24;
        ret = ee|dd|cc|bb|aa;
        return ret;
    }
    
     private static int DecodeUtfCodeSixChar(byte a, byte b, byte c , byte d, byte e, byte f)
    {
        int ret = 0;
        int ff = (f&0x3f);
        int ee = (e&0x3f)<<6;
        int dd = (d&0x3f)<<12;
        int cc = (c&0x3f)<<18;
        int bb = (b&0x3f)<<24;
        int aa = (a&0x1)<<30;
        ret = ff|ee|dd|cc|bb|aa;
        return ret;
    }
    
    private static bool IsEmoji(int codePoint)
    {
        return ((codePoint >= 0x1f000) && (codePoint <= 0x1f6f0))
        || ((codePoint >= 0x2000) && (codePoint <= 0x2940))
        || ((codePoint >= 0xE000) && (codePoint <= 0xE4FF));
    }
    
}
