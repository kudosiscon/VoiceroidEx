using System;

using saga.file;

namespace saga.voiceroid
{
    class VoiceroidEx
    {
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
                //VoiceroidInfo info = VoiceroidFactory4Win7.CreateAoi();
                //VoiceroidInfo info = VoiceroidFactory4Win7.CreateAkane();
                VoiceroidInfo info = VoiceroidFactory4Win7.Create(ri.GetVoiceroidType());
                VoiceroidNotify voiceroid = new VoiceroidNotify4Win7("dic/ipadic", info);

				// 保存時に上書きを設定
				voiceroid.SetForceOverWriteFlag(ri.GetForceOverWriteFlag());
				// デバッグ表示フラグ設定
				//voiceroid.SetDebugFlag(ri.GetDebugFlag());
                voiceroid.SetDebugFlag(true);

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
