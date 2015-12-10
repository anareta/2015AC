using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace WPFApp.Models
{
    /// <summary>
    /// 人管理クラス
    /// </summary>
    public class PeopleManager : BindableBase
    {
        /// <summary>
        /// データ本体
        /// </summary>
        private ObservableCollection<PersonModel> PeopleSource { get; set; }

        /// <summary>
        /// 名鑑
        /// </summary>
        public ReadOnlyObservableCollection<PersonModel> People { get; private set; }

        /// <summary>
        /// 人の数
        /// </summary>
        public int Count
        {
            get
            {
                return this.PeopleSource.Count;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PeopleManager()
        {
            this.PeopleSource = new ObservableCollection<PersonModel>();
            this.People = new ReadOnlyObservableCollection<PersonModel>(this.PeopleSource);

            this.PeopleSource.Add(new PersonModel() { Name = "日向縁", Age = 15, BirthDay = new Date(11, 11), Note = "CV：種田梨沙" });
            this.PeopleSource.Add(new PersonModel() { Name = "野々原ゆずこ", Age = 16, BirthDay = new Date(3, 24), Note = "CV：大久保瑠美" });
            this.PeopleSource.Add(new PersonModel() { Name = "櫟井唯", Age = 15, BirthDay = new Date(5, 1), Note = "CV：津田美波" });
            this.PeopleSource.Add(new PersonModel() { Name = "相川千穂", Age = 14, BirthDay = new Date(10, 5), Note = "CV：茅野愛衣" });
            this.PeopleSource.Add(new PersonModel() { Name = "岡野佳", Age = 12, BirthDay = new Date(2, 21), Note = "CV：潘めぐみ" });
            this.PeopleSource.Add(new PersonModel() { Name = "長谷川ふみ", Age = 18, BirthDay = new Date(12, 11), Note = "CV：清水茉菜" });

            var rx = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => h.Invoke,
                h => this.PeopleSource.CollectionChanged += h,
                h => this.PeopleSource.CollectionChanged -= h);

            rx.Subscribe(x =>
                {
                    this.OnPropertyChanged(() => this.Count);
                });

        }

        /// <summary>
        /// 人を削除します
        /// </summary>
        /// <param name="model"></param>
        public void Remove(PersonModel model)
        {
            var target = this.PeopleSource.FirstOrDefault(c => c.ID == model.ID);
            if (target == default(PersonModel))
            {
                return;
            }

            var index = this.PeopleSource.IndexOf(target);
            this.PeopleSource.RemoveAt(index);
        }

        /// <summary>
        /// 人を置き換えます
        /// </summary>
        /// <param name="model"></param>
        public void Edit(PersonModel model)
        {
            var target = this.PeopleSource.FirstOrDefault(c => c.ID == model.ID);
            if (target == default(PersonModel))
            {
                return;
            }

            var index = this.PeopleSource.IndexOf(target);
            this.PeopleSource.RemoveAt(index);
            this.PeopleSource.Insert(index, model);
        }

        /// <summary>
        /// 末尾に人を追加します
        /// </summary>
        /// <param name="model"></param>
        internal void Add(PersonModel model)
        {
            this.PeopleSource.Add(model);
        }
    }
}
