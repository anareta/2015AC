using System;
using System.Windows.Data;
using WPFApp.Models;

namespace WPFApp.Views.Converters
{
    /// <summary>
    /// Date構造体とDateTime構造体を相互に変換します
    /// </summary>
    public class DateDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Dateの確定値
        /// </summary>
        private Date _DateOld;

        /// <summary>
        /// DateTimeの確定値
        /// </summary>
        private DateTime _DateTimeOld;

        /// <summary>
        /// バインディング ソースの値をバインディング ターゲットに変換
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値。</param>
        /// <param name="targetType">無視します。</param>
        /// <param name="parameter">無視します。</param>
        /// <param name="culture">無視します。</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Date))
            {
                // エラーのときは前の値に戻す
                return this._DateTimeOld;
            }
            var d = (Date)value;
            this._DateOld = d;
            return d.ToDateTime();
        }

        /// <summary>
        /// バインディング ターゲットの値をバインディング ソースに変換
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値。</param>
        /// <param name="targetType">無視します。</param>
        /// <param name="parameter">無視します。</param>
        /// <param name="culture">無視します。</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DateTime))
            {
                // エラーのときは前の値に戻す
                return this._DateOld;
            }
            var dt = (DateTime)value;
            this._DateTimeOld = dt;
            return new Date(dt);
        }
    }
}
