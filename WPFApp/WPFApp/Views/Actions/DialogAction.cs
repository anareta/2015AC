using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Interactivity;

namespace WPFApp.Views.Actions
{
    /// <summary>
    /// ダイアログを表示します
    /// </summary>
    public class DialogAction : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// 呼び出し
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            // イベント引数とContextを取得する
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;

            // ContextのConfirmedに結果を格納する
            ctx.Confirmed = MessageBox.Show(
                args.Context.Content.ToString(),
                args.Context.Title,
                MessageBoxButton.OKCancel) == MessageBoxResult.OK;

            // コールバックを呼び出す
            args.Callback();
        }
    }
}
