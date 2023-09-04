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

- [ ] 186 - 194 

- [ ] 197 - 202 

- [ ] 205 - 208 

- [ ] 210 - 211

- [ ] 213 - 243

- [ ] 245 - 246 

- [x] 248 

- [x] 250 

- [x] 256 - 258 

- [x] 260

### 战斗动画文件名修复

- [ ] 1 - 43

- [ ] 45 - 53 

- [ ] 55 - 62 

- [ ] 64 - 75 

- [ ] 77 - 130 

- [ ] 132 - 173 

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

- [ ] 1 - 47  

- [ ] 51 - 56 

- [ ] 61 - 69 

- [ ] 72 - 79 

- [ ] 83 - 96 

- [ ] 98 - 107 

- [ ] 110 - 119 

- [ ] 121 - 135 

- [ ] 138 - 147 

- [ ] 149 - 151 

- [ ] 153 - 174 

- [ ] 177

- [ ] 179 

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
|                |                      |     |     |