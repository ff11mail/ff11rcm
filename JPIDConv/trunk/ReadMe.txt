
.NET Framework 2.0が必要です
意見要望はWindowerJP板にどうぞ

使い方

1. windower\plugins\spellcast\ にdllとexeをぶち込む
2. 適当に日本語でXMLを書く(UTF-8で)
3. XMLのファイル名を"default_jp.xml"や"THF_jp.xml"のように"_jp"をつけて保存する
   (windower\plugins\spellcast\ に保存)
4. exeを実行する
5. default.xml や THF.xmlが一括生成される

注意：
* 1回目の実行時にテーブルキャッシュを生成するのでちょっと時間かかる
* アイテム名は XML タグで囲まれている必要がある 例: <main>マンダウ</main> <var name="MainWeapon">マンダウ</var>
* 出力 xml がすでに存在しており、_jp.xml よりも新しい場合変換をスキップする(古い場合は上書きする)
* _en.xmlがあると日本語のxmlに変換し _en2jp.xml を出力する
* GPLでソースを公開しています。ソースの取り扱いはGPLを遵守するようにお願いします。

Tips：
 * <action type="Command">wait 1;input /equip sub アイテム</action> のようにしたいけど変換されない
  → <var name="SubWeapon">グラットンソード</var> と定義しておいて、input /equip main $SubWeapon などとすればOK

JPIDConvはRCMのサブプロジェクトです。
http://ff11rcm.googlecode.com/


履歴:
Version: 1.2.2
 * FFXIがインストールされてない環境での動作を修正(cacheがあれば動くようにした)
 * デバッグメッセージの出力を抑制

Version: 1.2.1
 * POLUtils の文字化けアイテム名対策
 * GPL2で公開

Version: 1.2.0
 * .NET 2.0でリコンパイル
 * ItemIDにNULLが含まれる場合の対処 (Thanks to ID:uRrrnn6g)

Version: 1.0.1
 * 初回リリース