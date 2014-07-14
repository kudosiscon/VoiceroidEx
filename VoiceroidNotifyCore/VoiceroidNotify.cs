using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using saga.util;
using NMeCab;

namespace saga.voiceroid
{
	/*
	 * Voiceroidコマンドライン拡張
	 * @author saga(@saga_dash)
	 */
	public abstract class VoiceroidNotify
	{
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		protected static extern IntPtr SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		protected static extern IntPtr SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, string lParam);
		[DllImport("user32.dll", EntryPoint = "PostMessage")]
		protected static extern IntPtr PostMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", EntryPoint = "PostMessage")]
		protected static extern IntPtr PostMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, string lParam);
		[DllImport("user32.dll", EntryPoint = "SetFocus")]
		protected static extern IntPtr SetFocus(IntPtr hWnd);
		[DllImport("user32.dll", EntryPoint = "SetWindowText")]
		protected static extern Boolean SetWindowText(IntPtr hWnd, string lpString);
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		protected static extern IntPtr SendMessageTimeout(
			IntPtr windowHandle,
			uint Msg,
			IntPtr wParam,
			string lParam,
			SendMessageTimeoutFlags flags,
			uint timeout,
			out IntPtr result);
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		protected static extern IntPtr SendMessageTimeout(
			IntPtr windowHandle,
			uint Msg,
			IntPtr wParam,
			IntPtr lParam,
			SendMessageTimeoutFlags flags,
			uint timeout,
			out IntPtr result);

		protected enum SendMessageTimeoutFlags : uint
		{
			SMTO_NORMAL = 0x0,
			SMTO_BLOCK = 0x1,
			SMTO_ABORTIFHUNG = 0x2,
			SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
			SMTO_ERRORONEXIT = 0x0020
		}
		protected static IntPtr WM_COMMAND = new IntPtr(273);
		protected static IntPtr ALLSELECT = new IntPtr(60);
		protected static IntPtr CUT = new IntPtr(52);
		protected static IntPtr PASTE = new IntPtr(56);
		protected static IntPtr WM_NULL = new IntPtr(0);
		protected static IntPtr ERROR = new IntPtr(-1);
		protected static IntPtr CB_ADDSTRING = new IntPtr(0x143);
		protected static IntPtr WM_KEYDOWN = new IntPtr(0x100);
		protected static IntPtr VK_DOWN = new IntPtr(0x28);
		protected static IntPtr WM_CLICK = new IntPtr(0xf5);

		// メインウィンドウ名
		protected string VOICEROID_TITLE;// = "VOICEROID＋ 結月ゆかり";
		// 保存ウィンドウ名
		protected string SAVE_WINDOW_TITLE;// = "音声ファイルの保存";
		// 強制上書きフラグ
		protected static bool forceOverWriteFlag;
		// デバッグフラグ
		protected static bool debugFlag;
        // LibNMeCab辞書のパス
        protected String dicPathFromExe;// = "dic/ipadic";
        // 一字あたりの読み上げ時間ms
        protected UInt32 interval;

		/*
		 * コンストラクタ
		 */
		public VoiceroidNotify():this("dic/ipadic") { }
        public VoiceroidNotify(String dicPathFromExe)
        {
			this.VOICEROID_TITLE = "VOICEROID＋ 結月ゆかり";
			this.SAVE_WINDOW_TITLE = "音声ファイルの保存";
			forceOverWriteFlag = false;
			debugFlag = false;
            this.dicPathFromExe = dicPathFromExe;
            this.interval = 180;
        }
		/*
		 * メインウィンドウ名の設定
		 * @param voiceroidWindowTitle メインウィンドウ名
		 */
		public void SetVoiceroidWindowTitle(String voiceroidWindowTitle)
		{
			this.VOICEROID_TITLE = voiceroidWindowTitle;
		}
		/*
		 * 保存ウィンドウ名を設定
		 * @param saveWindowTitle 保存ウィンドウ名
		 */
		public void SetSaveWindowTitle(String saveWindowTitle)
		{
			this.SAVE_WINDOW_TITLE = saveWindowTitle;
		}
		/*
		 * 強制上書き保存フラグを設定
		 * @param flag 強制上書き保存フラグ
		 */
		public void SetForceOverWriteFlag(bool flag)
		{
			forceOverWriteFlag = flag;
		}
		/*
         * デバッグ表示ON
		 * @param flag デバッグ表示フラグ
		 */
		public void SetDebugFlag(bool flag)
		{
			debugFlag = flag;
		}
        /*
         * LibNMeCab辞書のパスを設定
         * @param dicPathFromExe()
         */
        public void SetDicPathFromExe(String dicPathFromExe)
        {
            this.dicPathFromExe = dicPathFromExe;
        }
		// コンソール表示用
        /*
         * 一字あたりの読み上げ時間を設定
         * @param interval
         */
        public void SetInterval(UInt32 interval)
        {
            this.interval = interval;
        }
		public static void PrintDebug(string str)
		{
			if (!debugFlag)
			{
				return;
			}
			string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "log.log";
			StreamWriter writer = new StreamWriter(logFilePath, true, System.Text.Encoding.GetEncoding("Shift_JIS"));
			writer.WriteLine(str);
			writer.Close();
			Console.WriteLine(str);
		}
		// SendMessageTimout用Adaptor
		protected IntPtr SendMessageSub(IntPtr hWnd, IntPtr Msg, IntPtr wParam, string lParam)
		{
			IntPtr temp;
			return SendMessageTimeout(hWnd, (uint)Msg, wParam, lParam,
				SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 20000, out temp);
		}
		// SendMessageTimout用Adaptor
		protected IntPtr SendMessageSub(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam)
		{
			IntPtr temp;
			return SendMessageTimeout(hWnd, (uint)Msg, wParam, lParam,
				SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out temp);
		}
		/*
		 * メインウィンドウの再生ボタンハンドルを取得
		 * @param hWnd メインウィンドウハンドル
		 * @return 再生ボタンハンドル
		 */
        protected abstract IntPtr GetPlayButtonHandle(List<IntPtr> hWndList);
		/*
		 * 保存ボタンハンドルを取得
		 * @param hWnd メインウィンドウハンドル
		 * @return 保存ボタンハンドル
		 */
		protected abstract IntPtr GetOpenSaveWindowButtonHandle(List<IntPtr> hWndList);
		/*
		 * 保存ウィンドウのアドレスバーハンドルを取得
		 * @param hWnd 保存ウィンドウハンドル
		 * @return アドレスバーハンドル
		 */
		protected abstract IntPtr GetAddressToolbarHandle(List<IntPtr> hWndList);
		/*
		 * 保存ウィンドウのファイル名テキストボックスハンドルを取得
		 * @param hWnd 保存ウィンドウハンドル
		 * @return ファイル名テキストボックスハンドル
		 */
		protected abstract IntPtr GetFileNameTextBoxHandle(List<IntPtr> hWndList);
		/*
		 * 保存ウィンドウの保存ボタンハンドルを取得
		 * @param hWnd 保存ウィンドウハンドル
		 * @return 保存ボタンハンドル
		 */
		protected abstract IntPtr GetSaveButtonHandle(List<IntPtr> hWndList);
		/*
		 * テキストボックスに引数を設定
		 * @param talkStr 音声テキスト
		 * @throws ApplicationException Voiceroidが起動していません
		 * @return
		 */
		public abstract IntPtr SetPlayText(String talkStr);
		/*
		 * 音声再生
		 * @throws ApplicationException Voiceroidが起動していません
		 * @return
		 */
		public abstract IntPtr Play();
		/*
		 * SaveVoiceの実装
		 * @param pathStr 保存ファイルパス
		 * @throws ApplicationException Voiceroidが起動していません
		 * @return
		 */
		protected abstract IntPtr SaveVoiceImpl(String pathStr);
		/*
		 * 引数のパスに音声ファイルを保存
		 * @param pathStr 保存ファイルパス
		 * @throws ApplicationException Voiceroidが起動していません
		 * @return
		 */
		public IntPtr SaveVoice(String pathStr)
		{
			if (File.Exists(pathStr + ".wav"))
			{
				if (forceOverWriteFlag)
				{
					File.Delete(pathStr + ".wav");
				}
			}
			else if (Directory.Exists(pathStr))
			{
				throw new DirectoryNotFoundException("保存するファイルパスを指定してください");
			}
			return SaveVoiceImpl(pathStr);
		}
        /*
         * 再生時間を取得。音声テキストの長さから算出
         * @param talkStr 音声テキスト
         * @return 再生時間(ms)
         */
        protected int getInterval(String talkStr)
        {
            return getHiragana(talkStr).Length * (int)this.interval;
        }
        /*
         * 漢字から平仮名に変換
         * @param 音声テキスト
         * @return 平仮名文字列
         */
        protected String getHiragana(String talkStr)
        {
            MeCabParam param = new MeCabParam();
            param.DicDir = this.dicPathFromExe;
            MeCabTagger tagger = MeCabTagger.Create(param);
            MeCabNode node = tagger.ParseToNode(talkStr);
            String hiragana = "";
            while (node != null)
            {
                if (node.CharType > 0)
                {
                    //PrintDebug(node.Surface + "/t" + node.Feature);
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
            PrintDebug(hiragana);
            return hiragana;
        }


	}

}