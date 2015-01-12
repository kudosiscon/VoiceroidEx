using System;
using System.Collections.Generic;
using System.Text;

namespace saga.voiceroid
{
    public enum VoiceroidType
    {
        Yukari,
        YukariEx,
        //Maki,
        Akane,
        Aoi
    }
    public enum SystemType
    {
        Type1,
        Type2
    }
    public sealed class VoiceroidInfo
    {
        /*
         * コンストラクタ
         */
        public VoiceroidInfo() {}

        public VoiceroidInfo(VoiceroidType Type, SystemType SystemType, string VoiceroidTitle, string SaveWindowTitle,
            UInt32 Interval, int EditBoxIndex,int PlayButtonIndex, int OpenSaveWindowIndex, int AddressToolbarIndex,
            int FileNameTextBoxIndex, int SaveButtonIndex)
        {
            this.Type = Type;
            this.SType = SystemType;
            this.VoiceroidTitle = VoiceroidTitle;
            this.SaveWindowTitle = SaveWindowTitle;
            this.Interval = Interval;
            this.EditBoxIndex = EditBoxIndex; 
            this.PlayButtonIndex = PlayButtonIndex;
            this.OpenSaveWindowIndex = OpenSaveWindowIndex;
            this.AddressToolbarIndex = AddressToolbarIndex;
            this.FileNameTextBoxIndex = FileNameTextBoxIndex;
            this.SaveButtonIndex = SaveButtonIndex;
        }

        public VoiceroidType Type { get; set;}
        public string VoiceroidTitle  { get; set;}
        public SystemType SType { get; set; }
        public string SaveWindowTitle { get; set; }
        public UInt32 Interval { get; set; }
        public int EditBoxIndex { get; set; }
        public int PlayButtonIndex { get; set; }
        public int OpenSaveWindowIndex { get; set; }
        public int AddressToolbarIndex { get; set; }
        public int FileNameTextBoxIndex { get; set; }
        public int SaveButtonIndex { get; set; }

    }
}
