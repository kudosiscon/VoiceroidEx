VoiceroidExは「VOICEROID」をコマンドラインから読み上げ・保存を行うためのツールです
動画作成等を補助するため作成

Copyright 2013 @saga_dash

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


事前設定
	set.iniを実行ファイルと同じ階層に置く
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

2013/06/19	1.1	実行ファイルとライブラリを分離

2013/03/16	1.0	リリース
				Windows 7 のみ対応

