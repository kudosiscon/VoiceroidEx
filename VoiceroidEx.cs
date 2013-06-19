using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using saga.file;

namespace saga.voiceroid
{
    class VoiceroidEx
    {
		[STAThread]
		static void Main(string[] args)
		{
            try
			{
				// 引数多すぎ
				if (args.Length < 1 || 2 < args.Length)
				{
					throw new ArgumentException("引数を確認してください");
				}
				// やっつけINIファイルリーダー
				saga.file.ReadIniFile ri;
				try
				{
					// セッション[VOICEROID]を優先読み込み
					ri = new ReadIniFile("set.ini", "VOICEROID");
				}
				catch (Exception e)
				{
					// セッション[DEFAULT]を読み込み
					ri = new ReadIniFile("set.ini", "DEFAULT");
				}
				// インスタンス化
				VoiceroidNotify voiceroid = new VoiceroidNotify4Win7();
				voiceroid.SetVoiceroidWindowTitle(ri.GetMainWindowName());
				voiceroid.SetSaveWindowTitle(ri.GetSaveWindowName());
				voiceroid.SetForceOverWriteFlag(ri.GetForceOverWriteFlag());
				// デバッグ表示フラグ設定
				voiceroid.SetDebugFlag(ri.GetDebugFlag());

				// 音声テキストをテキストボックスに設定
				voiceroid.SetPlayText(args[0]);

				// 起動引数1: 音声を再生
				if (args.Length == 1)
				{
					voiceroid.Play();
				}
				// 起動引数2: 音声ファイルを保存
				else if (args.Length == 2)
				{
                    voiceroid.SaveVoice(args[1]);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine("エラーが発生しました");
				Console.WriteLine("何かキーを押してください");
				Console.ReadLine();
			}
		}
    }

}
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

		/*
		 * コンストラクタ
		 */
		public VoiceroidEx()
		{
			this.VOICEROID_TITLE = "VOICEROID＋ 結月ゆかり";
			this.SAVE_WINDOW_TITLE = "音声ファイルの保存";
			forceOverWriteFlag = false;
			debugFlag = false;
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
		/* デバッグ表示ON
		 * @param flag デバッグ表示フラグ
		 */
		public void SetDebugFlag(bool flag)
		{
			debugFlag = flag;
		}
		// デバッグ
		protected void DebugWriteLine(String message)
		{
			Debug.WriteLine("###############");
			Debug.WriteLine(message);
			Debug.WriteLine("###############");
			PrintDebug("###############");
			PrintDebug(message);
			PrintDebug("###############");
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


		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				// 引数多すぎ
				if (3 <= args.Length)
				{
					throw new ArgumentException("引数を確認してください");
				}
				// やっつけINIファイルリーダー
				saga.file.ReadIniFile ri;
				try
				{
					// セッション[VOICEROID]を優先読み込み
					ri = new ReadIniFile("set.ini", "VOICEROID");
				}
				catch (Exception e)
				{
					// セッション[DEFAULT]を読み込み
					ri = new ReadIniFile("set.ini", "DEFAULT");
				}
				// インスタンス化
				VoiceroidEx voiceroid = new VoiceroidEx4Win7();
				voiceroid.SetVoiceroidWindowTitle(ri.GetMainWindowName());
				voiceroid.SetSaveWindowTitle(ri.GetSaveWindowName());
				voiceroid.SetForceOverWriteFlag(ri.GetForceOverWriteFlag());
				// デバッグ表示フラグ設定
				voiceroid.SetDebugFlag(ri.GetDebugFlag());

				// 起動引数0: UDPサーバ起動
				if (args.Length == 0)
				{
					// 文字エンコードを設定 default:UTF8
					System.Text.Encoding enc = System.Text.Encoding.UTF8;
					// 受付ポートを設定
					System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient(ri.GetPort());

					while (true)
					{
						// 初期化
						System.Net.IPEndPoint remoteEP = null;
						// 受付待機
						byte[] rcvBytes = udp.Receive(ref remoteEP);
						// 文字列をエンコード
						string rcvMsg = enc.GetString(rcvBytes);

						PrintDebug("-----------------");
						PrintDebug("RecvData :" + rcvMsg);
						PrintDebug("SendAdd  :" + remoteEP.Address);
						PrintDebug("SendPort :" + remoteEP.Port);
						PrintDebug("-----------------");

						string exitStr = "--EXIT";
						if (rcvMsg.IndexOf(exitStr) == 0 && rcvMsg.Length == exitStr.Length)
						{
							return;
						}

						// オプション -filePath #filePath# を受信した場合、音声
						int index;
						string str = " -filePath ";
						if ((index = rcvMsg.IndexOf(str)) != -1)
						{
							// パス切り出し
							int length = rcvMsg.Length - index - str.Length;
							string filePath = rcvMsg.Substring(index + str.Length, length);
							// 音声テキスト切り出し
							rcvMsg = rcvMsg.Substring(0, index);

							// 音声テキストをテキストボックスに設定
							voiceroid.SetPlayText(rcvMsg);
							// 音声ファイルを保存
							voiceroid.SaveVoice(filePath);
							continue;
						}

						// 音声テキストをテキストボックスに設定
						voiceroid.SetPlayText(rcvMsg);
						// 再生
						voiceroid.Play();
						// 処理終了を送信
						udp.Send(rcvBytes, rcvBytes.Length, remoteEP.Address.ToString(), remoteEP.Port);
					}
				}
				// 音声テキストをテキストボックスに設定
				voiceroid.SetPlayText(args[0]);

				// 起動引数1: 音声を再生
				if (args.Length == 1)
				{
					voiceroid.Play();
				}
				// 起動引数2: 音声ファイルを保存
				else if (args.Length == 2)
				{
					voiceroid.SaveVoice(args[1]);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine("エラーが発生しました");
				Console.WriteLine("何かキーを押してください");
				Console.ReadLine();
			}
		}
	}

}