using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using LoadingWidget;

namespace TreeHoleBackStage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<DataTemplate> Datas { get; set; }
        private readonly MySqlConnection conn;
        readonly List<WritingWindow> writingWindows;

        public MainWindow()
        {
            InitializeComponent();
            Datas = new ObservableCollection<DataTemplate>();
            const string connectKey = "Database=tree_hole;Data Source=101.43.188.94;port=3306;User Id=root;Password=Ton@8177919;Charset=utf8";
            conn = new MySqlConnection(connectKey);
            writingWindows = new List<WritingWindow>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conn.Open();            

            dataList.ItemsSource = Datas;
            FreshData();
        }

        private void FreshData()
        {
            MySqlCommand cmd = new MySqlCommand("select * from main where answer like 'Have not answer yet,%'", conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            for (; Datas.Count > 0; Datas.RemoveAt(Datas.Count - 1)); // remove all of the elements
            
            while (reader.Read())
            {
                DataTemplate dataTemplate = new DataTemplate(writingWindows)
                {
                    ID = reader.GetString("user_id"),
                    Question = reader.GetString("question"),
                    Reply = reader.GetString("answer")
                };
                dataTemplate.OnReplied += DataTemplate_OnReplied;
                Console.WriteLine(dataTemplate);
                Datas.Add(dataTemplate);
            }
            reader.Close();
        }

        private void DataTemplate_OnReplied(string? id, string? reply)
        {
            string sql = "update main set answer='" + reply + "' where user_id='" + id + "'";
            MySqlCommand command = new MySqlCommand(sql, conn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure to exit this application?", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            conn.Close();
            System.Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (writingWindows.Count > 0)
            {
                MessageBox.Show("please close all of the editing windows before refreshing");
                return;
            }
            FreshData();
        }

        public class DataTemplate
        {
            public string? ID { get; set; }
            public string? Question { get; set; }
            public string? Reply { get; set; }

            private readonly List<WritingWindow> writingWindows;
            private WritingWindow? myWindow;

            public DataTemplate(List<WritingWindow> writingWindows)
            {
                this.writingWindows = writingWindows;
            }

            public void Start()
            {
                myWindow = new WritingWindow(ID, Question, Reply);
                writingWindows.Add(myWindow);
                myWindow.OnSubmited += Window_OnSubmited;
                myWindow.Show();
            }

            private void Window_OnSubmited(string? replyText)
            {
                Reply = replyText;
                TriggerReplyed();
                writingWindows.Remove(myWindow);
            }

            public delegate void ReplyHandler(string? id, string? replyText);
            public event ReplyHandler? OnReplied;
            protected void TriggerReplyed()
            {
                OnReplied?.Invoke(ID, Reply);
            }
        }

        private void dataList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataList.SelectedIndex >= 0)
                Datas[dataList.SelectedIndex].Start();
        }

        private void FreshAll()
        {
            MySqlCommand cmd = new MySqlCommand("select * from main", conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            for (; Datas.Count > 0; Datas.RemoveAt(Datas.Count - 1)) ; // remove all of the elements

            while (reader.Read())
            {
                DataTemplate dataTemplate = new DataTemplate(writingWindows);
                try {
                    dataTemplate.ID = reader.GetString("user_id");
                } catch (System.Data.SqlTypes.SqlNullValueException){}
                try {
                    dataTemplate.Question = reader.GetString("question");
                } catch (System.Data.SqlTypes.SqlNullValueException){}
                try {
                    dataTemplate.Reply = reader.GetString("answer");
                } catch (System.Data.SqlTypes.SqlNullValueException){}

                dataTemplate.OnReplied += DataTemplate_OnReplied;
                Console.WriteLine(dataTemplate);
                Datas.Add(dataTemplate);
            }
            reader.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (writingWindows.Count > 0)
            {
                MessageBox.Show("please close all of the editing windows before refreshing.");
                return;
            }
            FreshAll();
        }
    }
}
