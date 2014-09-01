using System;
using System.Collections.Generic;

using saga.util;

namespace saga.voiceroid
{
	/*
	 * Voiceroidコマンドライン拡張 for Windows XP
	 * ToDo: SaveVoiceImpl
	 * @author saga(@saga_dash)
	 */
	public class VoiceroidNotify4WinXP : VoiceroidNotify
	{
		public VoiceroidNotify4WinXP() : base() { }

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
			int retryTimes = 3;
			int retryDelay = 100;
			System.Windows.Forms.Clipboard.SetDataObject(talkStr, true, retryTimes, retryDelay);
			System.Threading.Thread.Sleep(100);
			// メインウィンドウにコマンドを送りテキストを貼り付け
			return SendMessageSub(hWndMain, WM_COMMAND, PASTE, WM_NULL);
		}
		public override IntPtr Play()
		{
            saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);
			IntPtr hTalkButton = GetPlayButtonHandle(mainWndSearch.GetWindowHandleList());

			PrintDebug("---play---");
			PrintDebug("hTalkButton: " + hTalkButton.ToString("X"));
			PrintDebug("----------");

			// VOCEROID ハングアップ用にTimeout設定し再生ボタン押
			return SendMessageSub(hTalkButton, WM_NULL, WM_NULL, WM_NULL);

		}
		// TODO: 保存ダイアログのコンボボックスにファイルパスの入力がうまくいかないので修正要
		protected override IntPtr SaveVoiceImpl(String pathStr)
		{
            saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.voiceroidInfo.VoiceroidTitle);
			IntPtr hOpenSaveWindowButton = GetOpenSaveWindowButtonHandle(mainWndSearch.GetWindowHandleList());

			PrintDebug("---saveVoice---");
			PrintDebug("hOpenSaveWindowButton: " + hOpenSaveWindowButton.ToString("X"));
			PrintDebug("---------------");

			// 保存ボタン押 保存ダイアログが立ち上がる
			PostMessage(hOpenSaveWindowButton, WM_NULL, WM_NULL, WM_NULL);

			System.Threading.Thread.Sleep(1500);

            saga.util.WindowHandleSearch saveWndSearch = new WindowHandleSearch(this.voiceroidInfo.SaveWindowTitle);

            IntPtr hWndSave = saveWndSearch.GetParentWindowHandle();
			IntPtr hFilenameTextBox = GetFileNameTextBoxHandle(saveWndSearch.GetWindowHandleList());
			IntPtr hSaveButton = GetSaveButtonHandle(saveWndSearch.GetWindowHandleList());

			PrintDebug("---saveVoice---");
			PrintDebug("hWndSave: " + hWndSave.ToString("X"));
			PrintDebug("hFilenameTextBox: " + hFilenameTextBox.ToString("X"));
			PrintDebug("hSaveButton: " + hSaveButton.ToString("X"));
			PrintDebug("---------------");

			// テキストボックスにフォーカスをあてる
			SetFocus(hFilenameTextBox);
			// Win7のダイアログはコンボボックス扱いなのでコンボボックスにPathを追加
			SendMessageSub(hFilenameTextBox, CB_ADDSTRING, WM_NULL, pathStr);
			// 下キーを送信しPathを表示
			SendMessageSub(hFilenameTextBox, WM_KEYDOWN, VK_DOWN, WM_NULL);
			/*				
			 static IntPtr WM_SETTEXT = new IntPtr(0x0c);
			 static IntPtr CB_SHOWDROPDOWN = new IntPtr(0x14f);
			 PostMessage(hFilenameTextBox, WM_SETTEXT, new IntPtr(0), pathStr); // WM_SETTEXT
			 PostMessage(hFilenameTextBox, CB_SHOWDROPDOWN, new IntPtr(1), WM_NULL); // リストを開く
			 System.Threading.Thread.Sleep(100);
			 PostMessage(hFilenameTextBox, CB_SHOWDROPDOWN, new IntPtr(0), WM_NULL); // リストを閉じる
			 System.Threading.Thread.Sleep(100);
			 */
			//		SetWindowText(hFilenameTextBox, pathStr);
			System.Threading.Thread.Sleep(100);

			// 保存ボタンクリック
			return PostMessage(hSaveButton, WM_CLICK, WM_NULL, WM_NULL);
		}

	}
}
