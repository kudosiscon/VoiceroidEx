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
		public VoiceroidNotify4Win7() : base() { }
		protected override IntPtr GetPlayButtonHandle(List<IntPtr> hWndList)
		{
			return hWndList[9];
		}
		protected override IntPtr GetOpenSaveWindowButtonHandle(List<IntPtr> hWndList)
		{
			return hWndList[7];
		}
		protected override IntPtr GetAddressToolbarHandle(List<IntPtr> hWndList)
		{
			/*
			foreach (IntPtr a in hWndList)
			{
				Debug.WriteLine(a.ToString("X"));
			}
			*/
			return hWndList[35];
		}
		protected override IntPtr GetFileNameTextBoxHandle(List<IntPtr> hWndList)
		{
			return hWndList[3];
		}
		protected override IntPtr GetSaveButtonHandle(List<IntPtr> hWndList)
		{
			/*
			foreach (IntPtr a in hWndList)
			{
				Debug.WriteLine(a.ToString("X"));
			}
			 */
			return hWndList[19];
		}
		public override IntPtr SetPlayText(String talkStr)
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.VOICEROID_TITLE);

			PrintDebug("---setTalkText---");
			PrintDebug("setText: " + talkStr);
			PrintDebug("-----------------");

			IntPtr hWndMain = mainWndSearch.GetList(-1);

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
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.VOICEROID_TITLE);
			IntPtr hTalkButton = GetPlayButtonHandle(mainWndSearch.GetList());

			PrintDebug("---play---");
			PrintDebug("hTalkButton: " + hTalkButton.ToString("X"));
			PrintDebug("----------");

			// VOCEROID ハングアップ用にTimeout設定し再生ボタン押
			return SendMessageSub(hTalkButton, WM_NULL, WM_NULL, WM_NULL);

		}
		protected override IntPtr SaveVoiceImpl(String pathStr)
		{
			saga.util.WindowHandleSearch mainWndSearch = new WindowHandleSearch(this.VOICEROID_TITLE);
			IntPtr hOpenSaveWindowButton = GetOpenSaveWindowButtonHandle(mainWndSearch.GetList());

			PrintDebug("---saveVoice---");
			PrintDebug("hOpenSaveWindowButton: " + hOpenSaveWindowButton.ToString("X"));
			PrintDebug("---------------");

			// 保存ボタン押 保存ダイアログが立ち上がる
			PostMessage(hOpenSaveWindowButton, WM_NULL, WM_NULL, WM_NULL);

			System.Threading.Thread.Sleep(1500);

			saga.util.WindowHandleSearch saveWndSearch = new WindowHandleSearch(this.SAVE_WINDOW_TITLE);

			IntPtr hWndSave = saveWndSearch.GetList(-1);
			IntPtr hFilenameTextBox = GetFileNameTextBoxHandle(saveWndSearch.GetList());
			IntPtr hSaveButton = GetSaveButtonHandle(saveWndSearch.GetList());

			PrintDebug("---saveVoice---");
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
