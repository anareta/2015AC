using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WPFApp.Models;

namespace WPFApp.ViewModels
{
    /// <summary>
    /// 詳細画面ViewModel
    /// </summary>
    public class DetailsViewModel : BindableBase, IInteractionRequestAware
    {
        /// <summary>
        /// インタラクションの終了
        /// </summary>
        public Action FinishInteraction { get; set; }

        /// <summary>
        /// リクエスト
        /// </summary>
        private INotification _Notification;
        
        /// <summary>
        /// リクエスト
        /// </summary>
        public INotification Notification
        {
            get { return this._Notification; }
            set
            {
                // MainWindowViewModelから渡されたデータを展開する
                this._Notification = value;

                // Modelクラスを設定
                this.Person = this._Notification.Content as PersonModel;
                this._Notification.Content = null;

                // ReactivePropertyのプロパティを設定
                this.Name = this.Person.ToReactivePropertyAsSynchronized(x => x.Name)
                                .AddTo(this.Disposable);
                this.Age = this.Person.ToReactivePropertyAsSynchronized(x => x.Age)
                               .AddTo(this.Disposable);
                this.BirthDay = this.Person.ToReactivePropertyAsSynchronized(x => x.BirthDay)
                                    .AddTo(this.Disposable);
                this.Note = this.Person.ToReactivePropertyAsSynchronized(x => x.Note)
                                .AddTo(this.Disposable);

                // 確定コマンド
                this.CommitCommand = this.Name
                                         .Select(s => !string.IsNullOrWhiteSpace(s)) // 名前欄が空欄でない場合に確定可能
                                         .Merge( this.Age.Select( a => a > -1) ) // 年齢が正の値の場合に確定可能
                                         .ToReactiveCommand();
                this.CommitCommand.Subscribe(_ => 
                    {
                        this._Notification.Content = this.Person;
                        this.FinishInteraction();
                    });

                // キャンセルコマンド
                this.CancelCommand = new ReactiveCommand();
                this.CancelCommand.Subscribe(_ => this.FinishInteraction());
            }
        }

        /// <summary>
        /// RxPの破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; set; }

        /// <summary>
        /// モデルクラス
        /// </summary>
        public PersonModel Person { get; private set; }

        /// <summary>
        /// 名前
        /// </summary>
        private ReactiveProperty<string> _Name;

        /// <summary>
        /// 名前
        /// </summary>
        public ReactiveProperty<string> Name
        {
            get { return this._Name; }
            set { this.SetProperty(ref this._Name, value); }
        }

        /// <summary>
        /// 年齢
        /// </summary>
        private ReactiveProperty<int> _Age;

        /// <summary>
        /// 年齢
        /// </summary>
        public ReactiveProperty<int> Age
        {
            get { return this._Age; }
            set { this.SetProperty(ref this._Age, value); }
        }

        /// <summary>
        /// 誕生日
        /// </summary>
        private ReactiveProperty<Date> _BirthDay;

        /// <summary>
        /// 誕生日
        /// </summary>
        public ReactiveProperty<Date> BirthDay
        {
            get { return this._BirthDay; }
            set { this.SetProperty(ref this._BirthDay, value); }
        }

        /// <summary>
        /// 備考
        /// </summary>
        private ReactiveProperty<string> _Note;

        /// <summary>
        /// 備考
        /// </summary>
        public ReactiveProperty<string> Note
        {
            get { return this._Note; }
            set { this.SetProperty(ref this._Note, value); }
        }

        /// <summary>
        /// 適用コマンド
        /// </summary>
        private ReactiveCommand _CommitCommand;

        /// <summary>
        /// 適用コマンド
        /// </summary>
        public ReactiveCommand CommitCommand
        {
            get{　return this._CommitCommand;　}
            private set　{　this.SetProperty(ref this._CommitCommand, value);　}
        }

        /// <summary>
        /// キャンセルコマンド
        /// </summary>
        private ReactiveCommand _CancelCommand;

        /// <summary>
        /// キャンセルコマンド
        /// </summary>
        public ReactiveCommand CancelCommand
        {
            get { return this._CancelCommand; }
            private set { this.SetProperty(ref this._CancelCommand, value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DetailsViewModel()
        {
            this.Disposable = new CompositeDisposable();
        }
    }
}
