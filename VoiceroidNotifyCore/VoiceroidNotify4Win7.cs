using System;
using System.Collections.Generic;

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

		public override IntPtr SetPlayText(String talkStr)
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);

			PrintDebug("---setTalkText---");
			PrintDebug("setText: " + talkStr);
			PrintDebug("-----------------");

			IntPtr hWndMain = mainWndSearch.GetParentWindowHandle();

			// メインウィンドウにコマンドを送りテキストを削除する
			SendMessageSub(hWndMain, WM_COMMAND, ALLSELECT, WM_NULL);
			SendMessageSub(hWndMain, WM_COMMAND, CUT, WM_NULL);

			// テキストをクリップボードに格納
			System.Threading.Thread.Sleep(100);
            talkString = talkStr;
            System.Threading.Thread t = new System.Threading.Thread(SetClipboard);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            System.Threading.Thread.Sleep(100);
			// メインウィンドウにコマンドを送りテキストを貼り付け
			return SendMessageSub(hWndMain, WM_COMMAND, PASTE, WM_NULL);
		}
        static void SetClipboard()
        {
            bool copy = true;
            int retryTimes = 100;
            int retryDelay=100;
            System.Windows.Forms.Clipboard.SetDataObject(talkString, copy, retryTimes, retryDelay);
        }
		public override IntPtr Play()
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);
			IntPtr hTalkButton = GetPlayButtonHandle(mainWndSearch.GetWindowHandleList());

			PrintDebug("---play---");
			PrintDebug("hTalkButton: " + hTalkButton.ToString("X"));
			PrintDebug("----------");

			// VOCEROID ハングアップ用にTimeout設定し再生ボタン押
            IntPtr result =  PostMessage(hTalkButton, WM_NULL, WM_NULL, WM_NULL);
            System.Threading.Thread.Sleep(getInterval(talkString)+100);
            return result;
		}
		protected override IntPtr SaveVoiceImpl(String pathStr)
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);
			IntPtr hOpenSaveWindowButton = GetOpenSaveWindowButtonHandle(mainWndSearch.GetWindowHandleList());

			PrintDebug("hOpenSaveWindowButton: " + hOpenSaveWindowButton.ToString("X"));

			// 保存ボタン押 保存ダイアログが立ち上がる
			PostMessage(hOpenSaveWindowButton, WM_NULL, WM_NULL, WM_NULL);

			System.Threading.Thread.Sleep(1500);

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
			System.Threading.Thread.Sleep(100);

			// 保存ボタンクリック
			return PostMessage(hSaveButton, WM_CLICK, WM_NULL, WM_NULL);
		}

	}
}
