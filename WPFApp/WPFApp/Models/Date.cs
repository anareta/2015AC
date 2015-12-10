using System;

namespace WPFApp.Models
{
    /// <summary>
    /// 日付構造体
    /// </summary>
    public struct Date
    {
        /// <summary>
        /// 月
        /// </summary>
        public uint _Month;

        /// <summary>
        /// 月
        /// </summary>
        public uint Month 
        { 
            get
            {
                return this._Month;
            }
            set
            {
                this._Month = value;
            }
        }

        /// <summary>
        /// 日
        /// </summary>
        public uint _Day;

        /// <summary>
        /// 日
        /// </summary>
        public uint Day
        {
            get
            {
                return this._Day;
            }
            set
            {
                this._Day = value;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public Date(uint month, uint day)
        {
            if (month > 12)
            {
                throw new ArgumentOutOfRangeException();
            }
            this._Month = month;

            if (day > 31)
            {
                throw new ArgumentOutOfRangeException();
            }
            this._Day = day;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="datetime"></param>
        public Date(DateTime datetime)
        {
            this._Month = (uint)datetime.Month;
            this._Day = (uint)datetime.Day;
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        public string String
        {
            get
            {
                return this._Month.ToString("D2") + "/" + this._Day.ToString("D2");
            }
        }

        /// <summary>
        /// DateTime構造体に変換
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(2000, (int)this._Month, (int)this._Day);
        }
    }
}
