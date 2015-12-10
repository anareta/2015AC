using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;
using WPFApp.Models;

namespace WPFApp.ViewModels
{
    /// <summary>
    /// MainWindowViewModelクラス
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region Properties

        /// <summary>
        /// 人データモデル
        /// </summary>
        public PeopleManager PeopleModel { get; private set; }

        /// <summary>
        /// 人データリスト
        /// </summary>
        public ReadOnlyReactiveCollection<PersonModel> People { get; private set; }

        /// <summary>
        /// 選択されているオブジェクト
        /// </summary>
        public ReactiveProperty<PersonModel> SelectedItem { get; private set; }


        /// <summary>
        /// データ追加コマンド
        /// </summary>
        public ReactiveCommand AddCommand { set; private get; }

        /// <summary>
        /// データ編集コマンド
        /// </summary>
        public ReactiveCommand EditCommand { set; private get; }

        /// <summary>
        /// データ削除コマンド
        /// </summary>
        public ReactiveCommand DeleteCommand { set; private get; }

        /// <summary>
        /// 詳細ダイアログを開くためのViewへの通知
        /// </summary>
        public InteractionRequest<Notification> OpenDetailsViewRequest { set; private get; }

        /// <summary>
        /// 確認ダイアログを開くためのViewへの通知
        /// </summary>
        public InteractionRequest<Confirmation> DialogRequest { set; private get; }


        #endregion Properties

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.PeopleModel = new PeopleManager();

            this.People = this.PeopleModel
                              .People
                              .ToReadOnlyReactiveCollection(x => (PersonModel)x.Clone());

            this.SelectedItem = new ReactiveProperty<PersonModel>();

            this.OpenDetailsViewRequest = new InteractionRequest<Notification>();
            this.DialogRequest = new InteractionRequest<Confirmation>();

            // コマンドの設定
            this.AddCommand = this.PeopleModel
                                  .ObserveProperty(x => x.Count) // ModelクラスのCountプロパティを監視
                                  .ToReactiveProperty()
                                  .Select(x => x < 11)           //　リストには10人まで追加可能
                                  .ToReactiveCommand();
            this.AddCommand
                .Select(_ => this.SelectedItem.Value) // 何かが選択されている場合に実行可能
                .Subscribe(_ => this.Add());

            this.EditCommand = this.SelectedItem
                               .Select(x => x != null)
                               .ToReactiveCommand();
            this.EditCommand
                .Select(_ => this.SelectedItem.Value)
                .Subscribe(x => this.Edit(x));

            this.DeleteCommand = this.SelectedItem
                                 .Select(x => x != null)　// 何かが選択されている場合に実行可能
                                 .ToReactiveCommand();
            this.DeleteCommand
                .Select(_ => this.SelectedItem.Value)
                .Subscribe(x => this.Delete(x));
        }

        #region Method

        /// <summary>
        /// データを追加
        /// </summary>
        private void Add()
        {
            var arg = new Notification()
            {
                Content = new PersonModel(),
                Title = "新規追加"
            };
            this.OpenDetailsViewRequest.Raise(
                        arg,
                        n => 
                        {
                            // ダイアログを閉じた後の処理
                            if (n.Content == null)
                            {
                                return;
                            }
                            this.PeopleModel.Add((PersonModel)n.Content);
                        });
        }


        /// <summary>
        /// データを編集
        /// </summary>
        private void Edit(PersonModel model)
        {
            if (model == null)
            {
                return;
            }

            var arg = new Notification()
            {
                Content = model.Clone(),
                Title = model.Name + " - 編集"
            };
            this.OpenDetailsViewRequest.Raise(
                        arg,
                        n =>
                        {
                            // ダイアログを閉じた後の処理
                            if (n.Content == null)
                            {
                                return;
                            }
                            this.PeopleModel.Edit((PersonModel)n.Content);
                        });
        }

        /// <summary>
        /// データを削除
        /// </summary>
        private void Delete(PersonModel model)
        {
            if (model == null)
            {
                return;
            }

            var arg = new Confirmation()
            {
                Content = "\"" + model.Name + "\"" + " を削除しますか？",
                Title = "削除"
            };
            this.DialogRequest.Raise(
                        arg,
                        n =>
                        {
                            // ダイアログを閉じた後の処理
                            if (n.Confirmed == false)
                            {
                                return;
                            }
                            this.PeopleModel.Remove(model);
                        });
        }

        #endregion Method
    }
}
