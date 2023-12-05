# 废都物语文件名修复

之前贴吧的汉化版因为文字编码的原因,日文字体会乱码,虽然不影响游玩,但是颇感不爽.
所以研究了下 EasyRPG 的一些有关 RPG MAKER 2000 的工具,准备自己折腾下.

2023.8.30

现在准备用 RM2000 一点点对比修复,之前本来想用 EasyRPG 的工具来进行修复,结果发现有很多问题(比如 导出的po文件无法编译成游戏需要的 ldb 文件),所以只好采用这样麻烦的方法进行修复

2023/8/31

可以使用 [Translator++](https://dreamsavior.net/download/)  解包游戏来进行汉化,如果解包的是汉化版会导致全部文本乱码 (日文版不会)

目前将原来的地图名全部换为了英文版的名称,为了能顺利读取地图图片,将一个符号删了.

详情可以看 脚本

日文 Wiki 参考:[ その他のアイテム - Ruina 廃都の物語 @ ウィキ - atwiki（アットウィキ）](https://w.atwiki.jp/ruinakokuryaku/pages/88.html)

感谢老哥的日语文件名修复脚本 : https://github.com/westernoon/Fix-garbled-Japanese-file-name-and-garbled-txt-text-content

RPGRewriter 是一个国外老哥整的工具,地址 [So You Want To Translate an RPG Maker 2000/2003 Game - vgperson's Posts](https://vgperson.com/posts.php?p=rpgmakerguide)

其他:  [RPG 制作大师 系列游戏安装教程 - 野比大雄的生化危机WIKI_BWIKI_哔哩哔哩](https://wiki.biligame.com/nobihaza/RPG_%E5%88%B6%E4%BD%9C%E5%A4%A7%E5%B8%88_%E7%B3%BB%E5%88%97%E6%B8%B8%E6%88%8F%E5%AE%89%E8%A3%85%E6%95%99%E7%A8%8B)

## TODO

### 地图名称修复

- [x] 48  ホルムの町戦場

- [x] 49  49ホルムの町戦場2

- [x] 24 23滝の洞窟 24竜の塔・上

- [x] 25 25竜の塔・下

- [x] 26 26宮殿

- [x] 28 28大廃墟

- [x] 27 27妖精の塔・下

- [x] 30  30妖精の塔・上

- [x] 31 31小人の塔・上

- [x] 32 32小人の塔・下

- [x] 34 34巨人の塔・外

- [x] 35 35巨人の塔・中

- [x] 37 36墓所

- [x] 38 37墓所玄室 38アーガデウム

- [x] 44 44戦場

- [x] 45 45潜入

- [x] 46 46星幽界

- [x] 47 47忘却界

- [x] 71  71異界

- [x] 39 39ラストバトル

### 怪物图片不显示修复

- [x] 1 - 16

- [x] 18 - 27

- [x] 29 - 42

- [x] 44 - 47

- [x] 49 - 56

- [x] 59 - 68

- [x] 71 - 83

- [x] 85

- [x] 88 - 98

- [x] 101 - 135

- [x] 140

- [x] 143 - 152

- [x] 154 - 161

- [x] 163 - 164

- [x] 168 - 174

- [x] 176 - 183

- [x] 186 - 194

- [x] 197 - 202

- [x] 205 - 208

- [x] 210 - 211

- [x] 213 - 243

- [x] 245 - 246

- [x] 248

- [x] 250

- [x] 256 - 258

- [x] 260

### 战斗动画文件名修复

- [x] 1 - 43

- [x] 45 - 53

- [x] 55 - 62

- [x] 64 - 75

- [x] 77 - 130

- [x] 132 - 173

- [x] 175 - 185

- [x] 187 - 191

- [x] 193

- [x] 195 - 197

- [x] 199 - 207

- [x] 209 - 211

- [x] 213

- [x] 215 - 217

- [x] 219 - 222

- [x] 224 - 229

### 战斗动画修复

- [x] 1 - 43

- [x] 45 - 53

- [x] 55 - 62

- [x] 64 - 75

- [x] 77 - 130

- [x] 132 - 173

- [ ] 175 - 185

- [ ] 187 - 191

- [ ] 193

- [ ] 195 - 197

- [ ] 199 - 207

- [ ] 209 - 211

- [ ] 213

- [ ] 215 - 217

- [ ] 219 - 222

- [ ] 224 - 229

### 任务事件修复

- [x] 1◇ｷｬﾗｽﾃｰﾀｽ表示 ◇显示人物状态

- [x] 2◆仲間雇用

- [x] 3◇顔グラ設定

- [x] 4◆仲間･ｹﾞｽﾄ強制解雇 ◆仲間·Guest强解雇

- [x] 5★マップ開放表ｻﾌﾞﾒON

- [x] 6　全員経験点獲得

- [x] 7◆仲間並び順ﾘﾌﾚｯｼｭ

- [x] 8　生死判定

- [x] 9◇ｷｬﾗｽﾃｰﾀｽ調査

- [x] 10☆ﾗﾝﾀﾞﾑｴﾝｶｳﾝﾄ

- [x] 11　町に帰還

- [x] 12　ダンジョンに入る

- [x] 13　ＢＧＭ演奏

- [x] 14◆ｹﾞｽﾄｷｬﾗ強制雇用

- [x] 15☆ﾗﾝﾀﾞﾑｱｲﾃﾑ入手

- [x] 16◇作業者選択

- [x] 17◇作業者選択死亡不可

- [x] 18　罠判定

- [x] 19　罠ダメージ

- [x] 20　休息

- [x] 21◆仲間強制雇用

- [x] 22　ツルハシ消滅判定

- [x] 23　ロープ消滅判定

- [x] 24◇全キャラスキル調査

- [x] 25　入店前処理

- [x] 26　出店処理

- [x] 27  ◇全スキル調査簡易版

- [x] 28  　仲間との会話

- [x] 29  ★ｱｲﾃﾑ等ﾃﾞｰﾀ整理確認

- [x] 30  ◇全キャラ能力調査

- [x] 31  ◇全ｷｬﾗ能力表HPのみ

- [x] 32  ◇今回獲得経験点計算

- [x] 33  　古代硬貨使用

- [x] 34  　日付進める

- [x] 35  　魚釣り

- [x] 36  ★シス設定ＢＧＭ戻す

- [x] 37  ◆一人ぼっちになるよ

- [x] 38  　食料腐敗数算出

- [x] 39  　扉

- [x] 40  　宝箱

- [x] 41  機敏判定：個別

- [x] 42  生存術判定：個別

- [x] 43  知覚（罠察知）：個別

- [x] 44  盗賊判定：個別

- [x] 45  腕力判定：個別

- [x] 46  古代知識判定：個別

- [x] 47  水泳(生存術判定:個別

- [x] 51 機敏所持判定

- [x] 52 生存術所持判定

- [x] 53 危険感知所持判定

- [x] 54 盗賊の技所持判定

- [x] 55 腕力所持判定

- [x] 56 古代知識所持判定

- [x] 61 機敏判定：全体

- [x] 62 生存術判定：全体

- [x] 63 危感（罠察知）：全体

- [x] 64 盗賊判定：全体

- [x] 65 腕力判定：全体

- [x] 66 古代知識判定：全体

- [x] 67 水泳（生存判定：全体

- [x] 68 伐採(腕力生存術:全体

- [x] 69 生存術(台詞無：全体

- [x] 72 水泳挑戦セリフ

- [x] 73 危機感知セリフ

- [x] 74 盗賊挑戦セリフ

- [x] 75 腕力挑戦セリフ

- [x] 76 古代知識挑戦セリフ

- [x] 77 汎用挑戦セリフ

- [x] 78 汎用成功セリフ

- [x] 79 汎用失敗セリフ

- [x] 83 イベント進行管理

- [x] 84 　コンフィグ

- [x] 85 　ラスボス戦前

- [x] 86 　ラスボス戦後

- [x] 87 　夢・休息イベント

- [x] 88 　竜石収集

- [x] 89 ★選択肢効果音

- [x] 90 ランダムアイテム1

- [x] 91 ランダムアイテム2

- [x] 92 ランダムアイテム3

- [x] 93 ランダムアイテム4

- [x] 94 ランダムアイテム5

- [x] 95 ランダムアイテム6

- [x] 96 　ランダム書籍

- [x] 98 　シーとアイテム争奪

- [x] 99 　シーフォン去る

- [x] 100 　呪い設定

- [x] 101 　呪い装備IDチェック

- [x] 102 　キレハ狼イベント

- [x] 103　メロダーク去る

- [x] 104 　キレハ去る

- [x] 105 　好感度トップ判定

- [x] 106 　好感度順並び替え

- [x] 107 　verup・デバッグ

- [x] 108 サブメニュー開き受付

- [x] 110 　料理使用

- [x] 111 　ダメ料理使用

- [x] 112 　暗黒料理使用

- [x] 113 　調合使用

- [x] 114　ﾗﾝﾌﾟを点ける(固定

- [x] 115 　魔法の光を点ける

- [x] 116 　ツルハシ使用オフ

- [x] 117 　ロープ使用オフ

- [x] 118 　武器鍛冶使用

- [x] 119 　防具鍛冶使用

- [x] 121　御使いの召喚

- [x] 122 　獣呼び

- [x] 123 　人工精霊の召喚

- [x] 124 　プディングの召喚

- [ ] 125 　精霊の召喚

- [ ] 126 　とげとげ魔神の召喚

- [ ] 127 　衛生兵

- [x] 128 　釣りオフ

- [x] 129 　世界地図使用

- [x] 130　ダウジング使用

- [ ] 131　妖精族のメモ

- [x] 132　冒険者の手帳使用

- [x] 133　忍び足使用

- [ ] 134　封印箱

- [ ] 135　忘れじの石

- [ ] 138 5パリス

- [ ] 139  6ネル

- [ ] 140 7ラバン

- [ ] 141 8キレハ

- [ ] 142 9シーフォン

- [ ] 143 10テレージャ

- [ ] 144 11アルソン

- [ ] 145 12エンダ

- [ ] 146 13フラン

- [ ] 147 14メロダーク

- [ ] 149 エンダ食料

- [ ] 150 わんこ食料

- [ ] 151 テンプレ

- [x] 153 乱数マップ開始

- [ ] 154 次の階層へ

- [ ] 155 乱数マップ新階層入る

- [x] 156 乱数マップ終了

- [x] 157 ★乱数ﾏｯﾌﾟ表示ｻﾌﾞﾒON

- [ ] 158 　乱数ﾏｯﾌﾟ敵ｴﾝｶｳﾝﾄ

- [x] 159　お宝貴重度LVで設定

- [x] 160　罠設定

- [x] 161　侵入時メッセ

- [x] 162　乱数ﾏｯﾌﾟｲﾍﾞﾝﾄ分岐

- [x] 163　乱数ﾏｯﾌﾟ経験値算出

- [ ] 164　　0集会場

- [ ] 165　　1倉庫

- [ ] 166　　2牢獄

- [ ] 167　　3罠の部屋

- [ ] 168　　4寝室

- [ ] 169　　5宝物庫

- [ ] 170　　6祭壇

- [ ] 171　　7泉

- [ ] 172　　8宿屋

- [ ] 173　　9不思議な人物

- [ ] 174  　 10ボス

- [ ] 177　　特殊イベント

- [x] 179  次の階層へ強制移動

### 需要修复的重要事件

#### ★

- [x] 5 ★マップ開放表ｻﾌﾞﾒON

- [x] 29 ★ｱｲﾃﾑ等ﾃﾞｰﾀ整理確認

- [x] 36 ★シス設定ＢＧＭ戻す

- [x] 89 ★選択肢効果音

- [ ] 157 ★乱数ﾏｯﾌﾟ表示ｻﾌﾞﾒON

- [ ] 177 　　特殊イベント

#### ◇

- [x] 3 ◇顔グラ設定

- [x] 9 ◇ｷｬﾗｽﾃｰﾀｽ調査

- [x] 16 ◇作業者選択

- [x] 17 ◇作業者選択死亡不可

- [x] 24 ◇全キャラスキル調査

- [x] 27 ◇全スキル調査簡易版

- [x] 30 ◇全キャラ能力調査

- [x] 31 ◇全ｷｬﾗ能力表HPのみ

- [x] 32 ◇今回獲得経験点計算

#### ☆

- [x] 10 ☆ﾗﾝﾀﾞﾑｴﾝｶｳﾝﾄ

- [x] 15 ☆ﾗﾝﾀﾞﾑｱｲﾃﾑ入手

#### ◆

- [x] 2 ◆仲間雇用

- [x] 4 ◆仲間･ｹﾞｽﾄ強制解雇

- [x] 7 ◆仲間並び順ﾘﾌﾚｯｼｭ

- [x] 14 ◆ｹﾞｽﾄｷｬﾗ強制雇用

- [x] 21 ◆仲間強制雇用

- [x] 37 ◆一人ぼっちになるよ

### 地图事件修复

- 23 滝の洞窟
  
  - [x] 滝01スタート
  
  - [x] 滝の洞窟07
  
  - [x] 滝12
  
  - [x] 滝の洞窟04
  
  - [x] 滝14
  
  - [x] 滝17付属水路岸ボツ？
  
  - [x] 滝の洞窟02
  
  - [x] マップ開放処理(ボツ
  
  - [x] 滝の洞窟03
  
  - [x] 滝の洞窟11
  
  - [x] 滝の洞窟11付属死体
  
  - [x] セーブ可能
  
  - [x] 滝の洞窟06
  
  - [x] 滝の洞窟09
  
  - [x] 滝の洞窟10
  
  - [x] 滝の洞窟05
  
  - [x] 滝08
  
  - [x] 滝13
  
  - [x] 滝17西小部屋
  
  - [x] 滝18
  
  - [x] 滝15出口
  
  - [x] 滝16
  
  - [x] 仲間と会話
  
  - [x] 滝の洞窟侵入時
  
  - [x] テスト
  
  - [x] 滝13付属狼イベ
  
  - [x] プロトタイプ (prototype)

- 03 ホルムの町
  
  - [x] EV0001
  - [x] EV0002
  - [x] 大河神殿
  - [x] 屋根裏部屋
  - [x] 広場
  - [x] 賢者の庵
  - [x] デバッグ用
  - [x] 町に入ったよイベント
  - [x] 主役変更
  - [x] 仲間と会話
  - [x] 港
  - [x] 墓地
  - [x] 宝１
  - [x] EV0016
  - [x] プロトタイプ
  - [x] EV0018
  - [x] EV0019
  - [x] EV0021
  - [x] EV0022
  - [x] 鍛冶屋 Smithy
  - [x] 占い屋
  - [x] 狼イベ敵 固定ID25
  - [x] EV0026
  - [x] EV0027
  - [x] EV0032

### 使用的变量替换

- [x] 1- 20
- [x] 21 - 40

### 存疑变量名

| 变量原文           | 英文翻译                 | 备注  | 编号  |
| -------------- | -------------------- | --- | --- |
| 今回の累計獲得経験点     | AccumulatedEXP Now   |     | 27  |
| 今朝の経験点         | TodayAccumulatedEXP  |     | 28  |
| 現在位置地形ID       | CurrentLocationID    |     | 30  |
| ﾁｪｯｸﾎﾟｲﾝﾄ間日数   | DayBetweenCheckpoint |     | 35  |
| ﾗﾝﾀﾞﾑｱｲﾃﾑ補正値   | RandomItemCorrection |     | 37  |
| ﾗﾝﾀﾞﾑｱｲﾃﾑ入手率   | RandomItemGet        |     | 38  |
| ﾗﾝﾀﾞﾑｱｲﾃﾑお宝貴重度 | RandomItemLevel      |     | 39  |
| 今日の資材採取度       | CollectionDegreeTody |     | 40  |
|                |                      |     |     |
|                |                      |     |     |
|                |                      |     | ’   |