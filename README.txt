VoiceroidExは「VOICEROID」をコマンドラインから読み上げ・保存を行うためのツールです
動画作成等を補助するため作成

本ソフトはNMeCab( http://sourceforge.jp/projects/nmecab/ )を使用しています。
別途ダウンロードし、実行フォルダに
・dic以下フォルダ
・LibNMeCab.dll
を配置してください。


Copyright 2014 @saga_dash

 Apache License Version 2.0（「本ライセンス」）に基づいてライセンスされます。あなたが
このファイルを使用するためには、本ライセンスに従わなければなりません。本ライセンスの
コピーは下記の場所から入手できます。

http://www.apache.org/licenses/LICENSE-2.0

 適用される法律または書面での同意によって命じられない限り、本ライセンスに基づいて
頒布されるソフトウェアは、明示黙示を問わず、いかなる保証も条件もなしに「現状のまま」
頒布されます。本ライセンスでの権利と制限を規定した文言については、本ライセンスを
参照してください。


製作者
	twitter: @saga_dash


動作確認環境
	Microsoft® Windows® 7
	.NET Framework 2.0
	NMeCab 0.06.4


事前設定
	set.ini と VoiceroidNotify.dllを実行ファイルと同じ階層に置く
	set.iniの設定内容は以下のとおり

	## 設定セクション、[DEFAULT]は修正用に変更しないことを推奨
	[VOICEROID]

	## 「VOICEROID」のウィンドウタイトル名
	## デフォルトは結月ゆかり
	MAIN_WINDOW_NAME=VOICEROID＋ 結月ゆかり

	## 保存ダイアログのウィンドウタイトル名
	SAVE_WINDOW_NAME=音声ファイルの保存

	## 上書きを許可する TRUE or FALSE
	FORCE_OVERWRITE=FALSE

	## デバッグ用フラグ TRUE or FALSE
	DEBUG=FALSE


動作条件
	「VOICEROID」を起動した状態でVoiceroidExを起動する
	コマンドプロンプトから起動
		$msg
		 :$msgを再生する
		
		$msg $path
		 :$msgの音声ファイルを$pathに出力する


	コマンド例：
		ゆかりさんまじ天使
		 :音声再生
		Twitterからお知らせ C:\a
		 :音声ファイルを C:\a に保存(ファイルはa.wav、a.txtが作成される)


開発者の方へ
Ver1.1よりメイン機能をライブラリ化しましたので、
コマンドラインから操作するだけでは足りない方は
VoiceroidNotify.dllを参照に追加して
VoiceroidEx/VoiceroidEx.cs
を参考にして組み込んでもらって構いません。

現状のままではVOICEROIDの設定が一人しか出来ない等
拡張性に乏しいので、、、


バージョン

2014/07/15	1.3	NMeCabLibを使用し漢字から平仮名に変換し、読み上げ時間を計算し
			待機する。

2014/03/30	1.2	スレッド部を修正

2013/06/19	1.1	実行ファイルとライブラリを分離

2013/03/16	1.0	リリース
				Windows 7 のみ対応

