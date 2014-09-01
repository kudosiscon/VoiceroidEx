using System;
using System.Collections.Generic;
using System.Text;

namespace saga.voiceroid
{
    public static class VoiceroidFactory4Win7
    {
        public static VoiceroidInfo CreateYukari()
        {
            return new VoiceroidInfo(VoiceroidType.Yukari,"VOICEROID＋ 結月ゆかり",
                "音声ファイルの保存",180,9,7,35,3,19);
        }
        public static VoiceroidInfo CreateMaki()
        {
            return new VoiceroidInfo(VoiceroidType.Maki, "VOICEROID＋ 弦巻マキ",
                "音声ファイルの保存", 180, 9, 7, 35, 3, 19);
        }
        public static VoiceroidInfo Create(VoiceroidType type)
        {
            switch (type)
            {
                case VoiceroidType.Yukari:
                    return CreateYukari();
                case VoiceroidType.Maki:
                    return CreateMaki();
                default:
                    return CreateYukari();
            }
        }
        public static VoiceroidInfo[] CreateAll()
        {
            VoiceroidInfo[] list = new VoiceroidInfo[Enum.GetNames(typeof(VoiceroidType)).Length];
            int index = 0;
            foreach (VoiceroidType type in Enum.GetValues(typeof(VoiceroidType)))
            {
                list[index] = Create(type);
                index++;
            }
            return list;

        }
    }
}
