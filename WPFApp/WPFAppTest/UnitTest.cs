using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RM.Friendly.WPFStandardControls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WPFApp.Models;

namespace WPFAppTest
{
    [TestClass]
    public class UnitTest
    {
        /// <summary>
        /// Friendly用APPクラス
        /// </summary>
        WindowsAppFriend _app;

        /// <summary>
        /// テスト対象に存在するMainWindowインスタンス
        /// </summary>
        dynamic _main;

        /// <summary>
        /// テスト開始前の処理
        /// ここで、テスト対象のプロセスにアタッチする
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // 外部プロセスにアタッチ
            this._app = new WindowsAppFriend(Process.Start("WPFApp.exe"));
            
            // MainWindowを取得
            this._main = this._app.Type<Application>().Current.MainWindow;
        }

        /// <summary>
        /// テスト終了後の処理
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // MainWindowを閉じる
            Process.GetProcessById(_app.ProcessId).CloseMainWindow();
            Process.GetProcessById(_app.ProcessId).Kill();
        }

        /// <summary>
        /// 「追加」ボタンを押すと項目の追加画面が出てくるかどうかをテスト
        /// </summary>
        [TestMethod]
        public void TestAddButton()
        {
            // ボタンを押す前は、Windowは１つだけなのを確認
            Assert.AreEqual(1, (int)this._app.Type<Application>().Current.Windows.Count);

            // ボタンの特定方法１：コントロールに指定された"Name"から特定
            var buttonAdd = new WPFButtonBase(_main.addButton);
            // クリック操作を非同期でエミュレート
            Async a = new Async();
            buttonAdd.EmulateClick(a);

            // 詳細ウィンドウが出てきたのを確認
            Assert.AreEqual(2, (int)this._app.Type<Application>().Current.Windows.Count);

            // "新規追加"というタイトルのウィンドウが見つからなかったらNG
            Assert.IsNotNull(WindowControl.IdentifyFromWindowText(this._app, "新規追加"));
        }

        /// <summary>
        /// 削除ボタンの活性状態をテスト
        /// </summary>
        [TestMethod]
        public void TestDeleteButton_Enable()
        {
            // 未選択状態のときは削除ボタンを押せないことをテスト
            AppVar mainVar = _app.Type<Application>().Current.MainWindow;

            // ボタンの特定方法２：コントロールに指定されたバインディングから特定
            var logicalTree = mainVar.LogicalTree();
            var deleteButton = new WPFButtonBase(logicalTree.ByBinding("DeleteCommand").Single());

            Assert.IsFalse(deleteButton.IsEnabled);

            // リストから選択する操作を実行
            // DataGridを取得
            var dataGrid = new WPFDataGrid(logicalTree.ByBinding("PeopleModel.People").Single());
            // DataGridの最初の行を取得して選択操作をエミュレート
            var row = dataGrid.GetRow(0);
            row.EmulateChangeSelected(true);

            // 削除ボタンを押せるようになっていればOK
            Assert.IsTrue(deleteButton.IsEnabled);
        }

        /// <summary>
        /// 編集ボタンの活性状態をテスト
        /// </summary>
        [TestMethod]
        public void TestEditButton_Enable()
        {
            // ボタンの特定方法３：相手プロセスでボタンを特定するstaticメソッドを実行する
            WindowsAppExpander.LoadAssembly(this._app, this.GetType().Assembly); // ←DLLインジェクション
            var targetButton = this._app.Type(this.GetType())
                                   .GetButton(this._app.Type<Application>().Current.MainWindow,
                                              "編集");
            var editButton = new WPFButtonBase(targetButton);

            Assert.IsFalse(editButton.IsEnabled);

            // リストから選択する操作を実行
            // DataGridを取得
            AppVar mainVar = this._app.Type<Application>().Current.MainWindow;
            var logicalTree = mainVar.LogicalTree();
            var dataGrid = new WPFDataGrid(logicalTree.ByBinding("PeopleModel.People").Single());
            // DataGridの最初の行を取得して選択操作をエミュレート
            var row = dataGrid.GetRow(0);
            row.EmulateChangeSelected(true);

            // 編集ボタンを押せるようになっていればOK
            Assert.IsTrue(editButton.IsEnabled);
        }

        /// <summary>
        /// 指定されたWindow配下を検索し、指定された文字列とContentが一致するボタンを取得します
        /// </summary>
        /// <param name="window">検索対象となるWindow</param>
        /// <param name="label">検索する文字列</param>
        /// <returns>一致したボタン</returns>
        static Button GetButton(Window window, string label)
        {
            var logicalTree = window.LogicalTree();
            return (Button)logicalTree.ByType<Button>().FirstOrDefault(c => 
                {
                    string content = c.Content as string;
                    return content == label;
                });
        }

        /// <summary>
        /// 名前を編集できるかテスト
        /// </summary>
        [TestMethod]
        public void TestEditName()
        {
            // テストのシナリオ
            // 1.リストの先頭を選択
            // 2.「編集」ボタンをクリック
            // 3.「名前」テキストボックスを書き換える
            // 4.「確定」ボタンを押す

            // 編集ボタンを取得
            AppVar mainVar = _app.Type<Application>().Current.MainWindow;
            var logicalTree = mainVar.LogicalTree();
            var editButton = new WPFButtonBase(logicalTree.ByBinding("EditCommand").Single());

            // 1.リストの先頭を選択
            var dataGrid = new WPFDataGrid(logicalTree.ByBinding("PeopleModel.People").Single());
            var row = dataGrid.GetRow(0);
            row.EmulateChangeSelected(true);
            // 編集対象の名前を退避
            string name = dataGrid.GetCellText(0, 0);

            // 2.「編集」ボタンをクリック
            editButton.EmulateClick(new Async());

            AppVar subWindow = WindowControl.IdentifyFromWindowText(this._app, name + " - 編集").AppVar;
            Assert.IsNotNull(subWindow);
            var subLogicalTree = subWindow.LogicalTree();


            // 名前入力テキストボックスを取得
            var nameTextBox = new WPFTextBox(subLogicalTree.ByBinding("Name.Value").Single());
            // 3.「名前」テキストボックスを書き換える
            nameTextBox.EmulateChangeText(name + "変更後");

            // 確定ボタンを取得
            var commitButton = new WPFButtonBase(subLogicalTree.ByBinding("CommitCommand").Single());
            // 4.「確定」ボタンを押す
            commitButton.EmulateClick(new Async());

            // ちゃんと名前が書き換わっていることを確認
            Assert.AreEqual(name + "変更後", dataGrid.GetCellText(0, 0));

            // ViewModelも書き換わっていることを確認
            Assert.AreEqual(name + "変更後", (string)this._main.DataContext.SelectedItem.Value.Name);
            
            // ついでにModelが書き換わっていることを確認
            Assert.AreEqual(name + "変更後", (string)this._main.DataContext.PeopleModel.PeopleSource[0]._Name);
        }

        /// <summary>
        /// 年齢をテキストボックスから編集できるかテスト
        /// </summary>
        [TestMethod]
        public void TestEditAgeByTextBox()
        {
            AppVar mainVar = _app.Type<Application>().Current.MainWindow;
            var logicalTree = mainVar.LogicalTree();
            var editButton = new WPFButtonBase(logicalTree.ByBinding("EditCommand").Single());

            var dataGrid = new WPFDataGrid(logicalTree.ByBinding("PeopleModel.People").Single());
            var row = dataGrid.GetRow(0);
            row.EmulateChangeSelected(true);
            string name = dataGrid.GetCellText(0, 0);

            editButton.EmulateClick(new Async());

            AppVar subWindow = WindowControl.IdentifyFromWindowText(this._app, name + " - 編集").AppVar;
            Assert.IsNotNull(subWindow);
            var subLogicalTree = subWindow.LogicalTree();

            // インジェクション
            // WPFToolKitのIntegerUpDownのように複数の標準コントロールを組み合わせたコントロールは
            // 取得できないため、VisualTreeからTextBox部分のみを取り出す
            WindowsAppExpander.LoadAssembly(this._app, GetType().Assembly);
            
            { //テキストボックスから編集
                var targetTextBox = this._app.Type(this.GetType())
                                        .GetAgeTextBox(subWindow);
                var ageTextBox = new WPFTextBox(targetTextBox);

                // 年齢を99に変更
                ageTextBox.EmulateChangeText("99");
            }

            // 確定
            var commitButton = new WPFButtonBase(subLogicalTree.ByBinding("CommitCommand").Single());
            commitButton.EmulateClick(new Async());

            // ちゃんと年齢が書き換わっていることを確認
            Assert.AreEqual("99", dataGrid.GetCellText(0, 1));
        }

        /// <summary>
        /// 「年齢」のテキストボックス部分のみを取得します
        /// </summary>
        /// <param name="window">詳細ウィンドウ</param>
        /// <returns>「年齢」のテキストボックス部分</returns>
        static TextBox GetAgeTextBox(Window window)
        {
            var logicalTree = window.LogicalTree();
            DependencyObject IntegerUpDown = logicalTree.ByBinding("Age.Value").Single();

            // IntegerUpDownのVisualTreeから検索
            return IntegerUpDown.Descendants<TextBox>().Single();
        }

        /// <summary>
        /// 年齢をスピンボタンから編集できるかテスト
        /// </summary>
        [TestMethod]
        public void TestEditAgeBySpinButton()
        {
            AppVar mainVar = _app.Type<Application>().Current.MainWindow;
            var logicalTree = mainVar.LogicalTree();
            var editButton = new WPFButtonBase(logicalTree.ByBinding("EditCommand").Single());

            var dataGrid = new WPFDataGrid(logicalTree.ByBinding("PeopleModel.People").Single());
            var row = dataGrid.GetRow(0);
            row.EmulateChangeSelected(true);
            string name = dataGrid.GetCellText(0, 0);

            // 元の年齢を取得
            string age = dataGrid.GetCellText(0, 1);

            editButton.EmulateClick(new Async());

            AppVar subWindow = WindowControl.IdentifyFromWindowText(this._app, name + " - 編集").AppVar;
            Assert.IsNotNull(subWindow);
            var subLogicalTree = subWindow.LogicalTree();

            // インジェクション
            // WPFToolKitのIntegerUpDownのように複数の標準コントロールを組み合わせたコントロールは
            // 取得できないため、VisualTreeからTextBox部分のみを取り出す
            WindowsAppExpander.LoadAssembly(this._app, GetType().Assembly);

            { // スピンボタンから編集
                var targetButtons = this._app.Type(this.GetType())
                                        .GetAgeSpinButton(subWindow);
                var up = new WPFButtonBase(targetButtons.Item1);
                var dw = new WPFButtonBase(targetButtons.Item2);

                // ５回Upする
                up.EmulateClick();
                up.EmulateClick();
                up.EmulateClick();
                up.EmulateClick();
                up.EmulateClick();
                // ２回Downする
                dw.EmulateClick();
                dw.EmulateClick();
            }

            // 確定
            var commitButton = new WPFButtonBase(subLogicalTree.ByBinding("CommitCommand").Single());
            commitButton.EmulateClick(new Async());

            // ちゃんと年齢が書き換わっている（元の値から+5、-2されている）ことを確認
            Assert.AreEqual((int.Parse(age) + 5 - 2).ToString() , dataGrid.GetCellText(0, 1));
        }

        /// <summary>
        /// 「年齢」のスピンボタン部分のみを取得します
        /// </summary>
        /// <param name="window">詳細ウィンドウ</param>
        /// <returns>
        /// item1：増やす方
        /// item2：減らす方
        /// </returns>
        static Tuple<ButtonBase, ButtonBase> GetAgeSpinButton(Window window)
        {
            var logicalTree = window.LogicalTree();
            DependencyObject IntegerUpDown = logicalTree.ByBinding("Age.Value").Single();

            // IntegerUpDownのVisualTreeから検索
            return new Tuple<ButtonBase, ButtonBase>(IntegerUpDown.Descendants<ButtonBase>().First(c => c.Name == "PART_IncreaseButton"),
                                                     IntegerUpDown.Descendants<ButtonBase>().First(c => c.Name == "PART_DecreaseButton"));
        }
    }
}
