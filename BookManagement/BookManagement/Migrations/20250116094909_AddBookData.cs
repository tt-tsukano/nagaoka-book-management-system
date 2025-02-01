using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddBookData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM Books;
        DBCC CHECKIDENT ('Books', RESEED, 0);
    ");

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Author", "PublishedYear", "IsDeleted", "BorrowedStatus", "BorrowedDate", "ReturnDate", "UserId" },
                values: new object[,]
                {
            {
                "人生を変える思考スイッチの切り替え方 アドラー心理学",
                "八巻 秀",
                2015,
                false,
                false,
                null,
                null,
                null
            },
            {
                "物語思考「やりたいこと」が見つからなくて悩む人のキャリア設計術",
                "古川 健介",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            {
                "誰がアパレルを殺すのか",
                "杉原 淳一, 染原 睦美",
                2017,
                false,
                false,
                null,
                null,
                null
            },
            {
                "役に立つアパレル業務の教科書",
                "久保 茂樹, 岡崎 平",
                2016,
                false,
                false,
                null,
                null,
                null
            },
            {
                "チェーンストアの実務原則・シリーズ仕入れと調達",
                "渥美 俊一",
                1985,
                false,
                false,
                null,
                null,
                null
            },
            {
                "ビジネス・エコノミクス",
                "伊藤 元重",
                2004,
                false,
                false,
                null,
                null,
                null
            },
            {
                "世界一ふざけた夢の叶え方",
                "ひすいこたろう, 菅野一勢, 柳田厚志",
                2014,
                false,
                false,
                null,
                null,
                null
            },
            {
                "ダンゼン得する 知りたいことがパッとわかる 会社のことがよくわかる本",
                "平井孝代",
                2014,
                false,
                false,
                null,
                null,
                null
            },
            {
                "勝てるデザイン",
                "前田高志",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "リアル店舗を救うのは誰か",
                "小野里寧晃",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            // 11-20
            {
                "ナイキ最強のDX戦略",
                "白土 孝",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "SEのための小売・サービス向けIoTの知識と技術",
                "安野元人",
                2019,
                false,
                false,
                null,
                null,
                null
            },
            {
                "この1冊ですべてわかる 新版 ITコンサルティングの基本",
                "克元亮",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "DX時代の最強PMOになる方法",
                "甲州 潤",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            {
                "UI/UXデザインの原則",
                "平石大祐",
                2020,
                false,
                false,
                null,
                null,
                null
            },
            {
                "小売の未来 新しい時代を生き残る10の「リテールタイプと消費者の問いかけ」",
                "ダグ・スティーブンス, 斎藤栄一郎",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "銀行崩壊とフィンテックの未来 金融、個人情報、IoTフィンテックですべてが変わる！！",
                "久田和広",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "失敗しないDX企画48のネタ！",
                "三浦一大",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "岩田さん Iwata-San 岩田聡はこんなことを話していた。",
                "ほぼ日刊イトイ新聞",
                2019,
                false,
                false,
                null,
                null,
                null
            },
            {
                "AIが職場にやってきた 機械まかせにならないための9つのルール",
                "ケビン・ルース, 田沢恭子",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            // 21-30
            {
                "イラストでわかる！ DXで変わる100の景色",
                "森戸裕一",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            {
                "マッキンゼーが解き明かす生き残るためのDX",
                "黒川通彦, 平山智晴, 松本拓也, 片山博順",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "はじめての上流工程をやり抜くための本",
                "三輪一郎",
                2008,
                false,
                false,
                null,
                null,
                null
            },
            {
                "イシューからはじめよ 知的生産のシンプルな本質",
                "安宅和人",
                2010,
                false,
                false,
                null,
                null,
                null
            },
            {
                "チームワーキング ―ケースとデータで学ぶ「最強チーム」のつくり方",
                "中原淳, 田中聡",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "アジャイル型 最高のチームで価値を実現するために プロジェクトマネジメント",
                "中谷公巳",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "変える技術・考える技術",
                "高松智史",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "コンサルが「最初の３年間」で学ぶコト 知らないと一生後悔する99のスキルと5の挑戦",
                "高松智史",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            {
                "「フェルミ推定」から始まる問題解決の技術",
                "高松智史",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "ロジカルシンキングを超える戦略思考 フェルミ推定の技術",
                "高松智史",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            // 31-40
            {
                "ビジネスフレームワーク図鑑 すぐ使える問題解決・アイデア発想ツール70",
                "株式会社アンド",
                2018,
                false,
                false,
                null,
                null,
                null
            },
            {
                "経営者になるためのノート",
                "柳井 正",
                2015,
                false,
                false,
                null,
                null,
                null
            },
            {
                "非クリエイターのためのクリエイティブ課題解決術",
                "齋藤太郎",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "Process Visionary デジタル時代のプロセス変革リーダー",
                "山本政樹, 大井悠",
                2019,
                false,
                false,
                null,
                null,
                null
            },
            {
                "なぜ、この会社に人が集まるのか",
                "丸山 勇一",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "リアル店舗は消えるのか？ 流通DXが開くマーケティング新時代",
                "一般財団法人リテールAI研究会",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "観察力の鍛え方 一流のクリエイターは世界をどう見ているのか",
                "佐渡島庸平",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "売上最小化、利益最大化の法則 ―利益率29％の経営の秘密",
                "木下勝寿",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "マーケターのように生きろ 「あなたが必要だ」と言われ続ける人の思考と行動",
                "井上大輔",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "SCRUM BOOT CAMP THE BOOK【増補改訂版】 スクラムチームではじめるアジャイル開発",
                "西村直人, 永瀬美穂, 吉羽龍太郎",
                2020,
                false,
                false,
                null,
                null,
                null
            },
            // 41-51（最後）
            {
                "プロが教えるマーケティングリサーチとデータ分析の基本",
                "中野崇",
                2018,
                false,
                false,
                null,
                null,
                null
            },
            {
                "More Effective Agile モア・エフェクティブ・アジャイル「ソフトウェアリーダー」になるための28の指標",
                "Steve McConnell",
                2020,
                false,
                false,
                null,
                null,
                null
            },
            {
                "不況に強いビジネスは北海道の「小売」に学べ",
                "白鳥和生",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "ＵＩデザイン必携 ユーザーインターフェースの設計と改善を成功させるために",
                "原田秀司",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "エンジニアが学ぶ在庫管理システムの「知識」と「技術」",
                "株式会社GeNEE DX/IT ソリューション事業部",
                2023,
                false,
                false,
                null,
                null,
                null
            },
            {
                "アジャイルサムライ ―達人開発者への道",
                "Jonathan Rasmusson, 近藤修平, 角掛拓未",
                2011,
                false,
                false,
                null,
                null,
                null
            },
            {
                "情報を活用して、思考と行動を進化させる",
                "田中志",
                2021,
                false,
                false,
                null,
                null,
                null
            },
            {
                "問題解決のためのデータ分析",
                "齋藤健太",
                2013,
                false,
                false,
                null,
                null,
                null
            },
            {
                "「静かな人」の戦略書 ―騒がしすぎるこの世界で内向型が静かな力を発揮する法",
                "ジル・チャン, 神崎朗子",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "コンサルが読んでる本 100+α",
                "並木裕太, 青山正明, 藤熊浩平, 白井英介",
                2022,
                false,
                false,
                null,
                null,
                null
            },
            {
                "ON!OFFICE 活性化のスイッチを生むオフィスデザイン",
                "フィールドフォー・デザインオフィス",
                2009,
                false,
                false,
                null,
                null,
                null
            }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Books");
        }
    }
}
