using System;

using NMeCab;

namespace saga.voiceroid
{
    class MeCabAdapter
    {
        static public String GetHiragana(String dicPathFromExe, String str){
            MeCabParam param = new MeCabParam();
            param.DicDir = dicPathFromExe;
            MeCabTagger tagger = MeCabTagger.Create(param);
            MeCabNode node = tagger.ParseToNode(str);
            String hiragana = "";
            while (node != null)
            {
                if (node.CharType > 0)
                {
                    String[] splitStrArray = node.Feature.Split(',');
                    String splitStr;
                    if (splitStrArray.Length < 9)
                    {
                        splitStr = node.Surface;
                    }
                    else
                    {
                        splitStr = splitStrArray[7];
                    }
                    hiragana = hiragana + splitStr;
                }
                node = node.Next;
            }
            return hiragana;
        }
    }
}
