using Prism.Mvvm;
using System;

namespace WPFApp.Models
{
    /// <summary>
    /// 人
    /// </summary>
    public class PersonModel : BindableBase, ICloneable
    {
        /// <summary>
        /// 識別番号
        /// </summary>
        public string ID { set; get; }

        /// <summary>
        /// 名前
        /// </summary>
        private string _Name;

        /// <summary>
        /// 名前
        /// </summary>
        public string Name
        {
            get { return this._Name; }
            set { this.SetProperty(ref this._Name, value); }
        }

        /// <summary>
        /// 年齢
        /// </summary>
        private int _Age;

        /// <summary>
        /// 年齢
        /// </summary>
        public int Age
        {
            get { return this._Age; }
            set { this.SetProperty(ref this._Age, value); }
        }

        /// <summary>
        /// 誕生日
        /// </summary>
        private Date _BirthDay;

        /// <summary>
        /// 誕生日
        /// </summary>
        public Date BirthDay
        {
            get { return this._BirthDay; }
            set { this.SetProperty(ref this._BirthDay, value); }
        }

        /// <summary>
        /// 備考
        /// </summary>
        private string _Note;

        /// <summary>
        /// 備考
        /// </summary>
        public string Note
        {
            get { return this._Note; }
            set { this.SetProperty(ref this._Note, value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PersonModel()
        {
            this.Name = "名前";
            this.Age = 0;
            this.BirthDay = new Date(1, 1);

            this.ID = Guid.NewGuid().ToString("N");
        }


        /// <summary>
        /// 複製を作成します
        /// </summary>
        /// <returns>このインスタンスの複製</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
