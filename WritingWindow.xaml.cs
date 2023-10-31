using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TreeHoleBackStage
{
    /// <summary>
    /// WritingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WritingWindow : Window
    {
        readonly string? id, question, reply;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IDLabel.Content += id;
            QuestionContainer.Text = question;
            ReplyContainer.Text = reply;
            ReplyText.SpellCheck.IsEnabled = true;
        }

        public WritingWindow(string? id, string? question, string? reply)
        {
            InitializeComponent();
            this.id = id;
            this.question = question;
            this.reply = reply;
        }

        public delegate void SubmitHandler(string replyText);

        private void Window_Closed(object sender, EventArgs e)
        {
            TextRange textRange = new TextRange(ReplyText.Document.ContentStart, ReplyText.Document.ContentEnd);
            TriggerSubmited(textRange.Text);
        }

        public event SubmitHandler? OnSubmited;
        protected virtual void TriggerSubmited(string replyText)
        {
            OnSubmited?.Invoke(replyText);
        }
    }
}
