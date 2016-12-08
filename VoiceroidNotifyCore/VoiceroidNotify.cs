using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;


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
        protected static IntPtr WM_PASTE = new IntPtr(0x302);

        // voiceroid情報
        protected VoiceroidInfo voiceroidInfo;
		// 強制上書きフラグ
		protected static bool forceOverWriteFlag;
		// デバッグフラグ
		protected static bool debugFlag;
        // LibNMeCab辞書のパス
        protected static String dicPathFromExe;// = "dic/ipadic";

		/*
		 * コンストラクタ
		 */
		public VoiceroidNotify():this("dic/ipadic") { }
        public VoiceroidNotify(String dicPathFromExe):this(dicPathFromExe, null)
        {
            VoiceroidInfo info = VoiceroidFactory4Win7.CreateYukari();
            this.voiceroidInfo = info;
        }
        public VoiceroidNotify(String dicPathFromExe, VoiceroidInfo info)
        {
            this.voiceroidInfo = info;
            VoiceroidNotify.forceOverWriteFlag = false;
            VoiceroidNotify.debugFlag = false;
            VoiceroidNotify.dicPathFromExe = dicPathFromExe;
        }
        /*
         * Voiceroid情報の設定
         * @param VoiceroidInfo Voiceroid情報
         */
        public void SetVoiceroidInfo(VoiceroidInfo info)
		{
			this.voiceroidInfo = info;
		}
		/*
		 * 強制上書き保存フラグを設定
		 * @param flag 強制上書き保存フラグ
		 */
		public void SetForceOverWriteFlag(bool flag)
		{
            VoiceroidNotify.forceOverWriteFlag = flag;
		}
		/*
         * デバッグ表示ON
		 * @param flag デバッグ表示フラグ
		 */
		public void SetDebugFlag(bool flag)
		{
            VoiceroidNotify.debugFlag = flag;
		}
        /*
         * LibNMeCab辞書のパスを設定
         * @param dicPathFromExe
         */
        public void SetDicPathFromExe(String dicPathFromExe)
        {
            VoiceroidNotify.dicPathFromExe = dicPathFromExe;
        }
        // コンソール表示用
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
         * メインウィンドウのエディットボックスハンドルを取得
         * @param hWnd メインウィンドウハンドル
         * @return エディットボックスハンドル
         */
        protected IntPtr GetEditBoxHandle(List<IntPtr> hWndList)
        {
            return hWndList[voiceroidInfo.EditBoxIndex];
        }
        /*
         * メインウィンドウの再生ボタンハンドルを取得
         * @param hWnd メインウィンドウハンドル
         * @return 再生ボタンハンドル
         */
        protected IntPtr GetPlayButtonHandle(List<IntPtr> hWndList)
        {
            return hWndList[voiceroidInfo.PlayButtonIndex];
        }
		/*
		 * 保存ボタンハンドルを取得
		 * @param hWnd メインウィンドウハンドル
		 * @return 保存ボタンハンドル
		 */
        protected IntPtr GetOpenSaveWindowButtonHandle(List<IntPtr> hWndList)
        {
            return hWndList[voiceroidInfo.OpenSaveWindowIndex];
        }		/*
		 * 保存ウィンドウのアドレスバーハンドルを取得
		 * @param hWnd 保存ウィンドウハンドル
		 * @return アドレスバーハンドル
		 */
        protected IntPtr GetAddressToolbarHandle(List<IntPtr> hWndList)
        {
            return hWndList[voiceroidInfo.AddressToolbarIndex];
        }		/*
		 * 保存ウィンドウのファイル名テキストボックスハンドルを取得
		 * @param hWnd 保存ウィンドウハンドル
		 * @return ファイル名テキストボックスハンドル
		 */
        protected IntPtr GetFileNameTextBoxHandle(List<IntPtr> hWndList)
        {
            return hWndList[voiceroidInfo.FileNameTextBoxIndex];
        }		/*
		 * 保存ウィンドウの保存ボタンハンドルを取得
		 * @param hWnd 保存ウィンドウハンドル
		 * @return 保存ボタンハンドル
		 */
        protected IntPtr GetSaveButtonHandle(List<IntPtr> hWndList)
        {
            return hWndList[voiceroidInfo.SaveButtonIndex];
        }		/*
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
            PrintDebug("---SaveVoice---");
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
            return getHiragana(talkStr).Length * (int)this.voiceroidInfo.Interval;
        }
        /*
         * 漢字から平仮名に変換
         * @param 音声テキスト
         * @return 平仮名文字列
         */
        protected String getHiragana(String talkStr)
        {
            String hiragana = MeCabAdapter.GetHiragana(VoiceroidNotify.dicPathFromExe, talkStr);
            PrintDebug(hiragana);
            return hiragana;
        }


	}

}