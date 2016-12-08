using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using saga.util;

namespace saga.voiceroid
{
	/*
	 * Voiceroidコマンドライン拡張 for Windows 7
	 * @author saga(@saga_dash)
	 */
	public class VoiceroidNotify4Win7 : VoiceroidNotify
	{
        private static String talkString="";
        public VoiceroidNotify4Win7() : base(){ }
		public VoiceroidNotify4Win7(String dicPathFromExe) : base(dicPathFromExe) { }
        public VoiceroidNotify4Win7(String dicPathFromExe,VoiceroidInfo info) : base(dicPathFromExe, info) { }

        private static string[] Formats = new string[]{
            DataFormats.Bitmap,
            DataFormats.Text,
            DataFormats.WaveAudio,
            DataFormats.FileDrop
        };
		public override IntPtr SetPlayText(String talkStr)
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);

			PrintDebug("---setTalkText---");
			PrintDebug("setText: " + talkStr);
			PrintDebug("-----------------");

			// メインウィンドウにコマンドを送りテキストを削除する
			IntPtr hWndMain = mainWndSearch.GetParentWindowHandle();
            if (voiceroidInfo.SType == SystemType.Type1)
            {
                SendMessageSub(hWndMain, WM_COMMAND, ALLSELECT, WM_NULL);
                SendMessageSub(hWndMain, WM_COMMAND, CUT, WM_NULL);
            }
            else
            {
                IntPtr hEdit = GetEditBoxHandle(mainWndSearch.GetWindowHandleList()); //GetMenu(hWndMain);
                SendMessageSub(hEdit, new IntPtr(0x304), WM_NULL, WM_NULL);
            }


			// テキストをクリップボードに格納
            KeyValuePair<String,object> kvp = new KeyValuePair<string,object>();
            Thread t = new Thread(delegate()
            {
                try { var obj = Clipboard.GetDataObject();
                foreach (string item in obj.GetFormats(true))
                {
                    if(Array.IndexOf(Formats,item) >= 0){
                        kvp = new KeyValuePair<string,object>(item, obj.GetData(item));
                        break;
                    }
                }
                }
                catch (Exception e) { };
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            talkString = talkStr;
            t = new Thread(SetClipboard);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(talkStr);
            t.Join();

			// メインウィンドウにコマンドを送りテキストを貼り付け
            IntPtr result;
            if (voiceroidInfo.SType == SystemType.Type1)
            {
                result = SendMessageSub(hWndMain, WM_COMMAND, PASTE, WM_NULL);
            }
            else
            {
                IntPtr hEdit = GetEditBoxHandle(mainWndSearch.GetWindowHandleList()); //GetMenu(hWndMain);
                result = SendMessageSub(hEdit, WM_PASTE, WM_NULL, WM_NULL);
            }

            // 元データをクリップボードに格納
            t = new Thread(SetClipboardWithKVP);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(kvp);
            t.Join();

            return result;
		}
        static void SetClipboard(Object obj)
        {
            bool copy = true;
            int retryTimes = 100;
            int retryDelay = 100;
            Clipboard.SetDataObject((String)obj, copy, retryTimes, retryDelay);
        }
        static void SetClipboardWithKVP(Object obj)
        {
            KeyValuePair<string, object> kvp = (KeyValuePair<string, object>)obj;
            try
            {
                if (kvp.Key == null)
                {
                    return;
                }
                Clipboard.SetData(kvp.Key, kvp.Value);
            }
            catch (Exception e) { };
        }
		public override IntPtr Play()
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);
			IntPtr hTalkButton = GetPlayButtonHandle(mainWndSearch.GetWindowHandleList());

			PrintDebug("---play---");
			PrintDebug("hTalkButton: " + hTalkButton.ToString("X"));
			PrintDebug("----------");

			// VOCEROID ハングアップ用にTimeout設定し再生ボタン押
            IntPtr Msg = voiceroidInfo.SType == SystemType.Type1 ? WM_NULL : WM_CLICK;
            IntPtr result = PostMessage(hTalkButton, Msg, WM_NULL, WM_NULL);
            Thread.Sleep(getInterval(talkString) + 100);
            return result;
		}
		protected override IntPtr SaveVoiceImpl(String pathStr)
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);
			IntPtr hOpenSaveWindowButton = GetOpenSaveWindowButtonHandle(mainWndSearch.GetWindowHandleList());

			PrintDebug("hOpenSaveWindowButton: " + hOpenSaveWindowButton.ToString("X"));

			// 保存ボタン押 保存ダイアログが立ち上がる
            IntPtr Msg = voiceroidInfo.SType == SystemType.Type1 ? WM_NULL : WM_CLICK;
            PostMessage(hOpenSaveWindowButton, Msg, WM_NULL, WM_NULL);

			Thread.Sleep(1500);

			saga.util.WindowHandleSearch saveWndSearch = new WindowHandleSearch(this.voiceroidInfo.SaveWindowTitle);

            IntPtr hWndSave = saveWndSearch.GetParentWindowHandle();
			IntPtr hFilenameTextBox = GetFileNameTextBoxHandle(saveWndSearch.GetWindowHandleList());
			IntPtr hSaveButton = GetSaveButtonHandle(saveWndSearch.GetWindowHandleList());

			PrintDebug("hWndSave: " + hWndSave.ToString("X"));
			PrintDebug("hFilenameTextBox: " + hFilenameTextBox.ToString("X"));
			PrintDebug("hSaveButton: " + hSaveButton.ToString("X"));
			PrintDebug("---------------");

			// テキストボックスにフォーカスをあてる
//			SetFocus(hFilenameTextBox);
			// Win7のダイアログはコンボボックス扱いなのでコンボボックスにPathを追加
			SendMessageSub(hFilenameTextBox, CB_ADDSTRING, WM_NULL, pathStr);
			// 下キーを送信しPathを表示
			SendMessageSub(hFilenameTextBox, WM_KEYDOWN, VK_DOWN, WM_NULL);
			Thread.Sleep(100);

			// 保存ボタンクリック
			return PostMessage(hSaveButton, WM_CLICK, WM_NULL, WM_NULL);
		}

	}
}
