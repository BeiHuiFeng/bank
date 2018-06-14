using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MySql.Data.MySqlClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace newBank {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class branchListItem {
        public string Name;
        public string Num;
        public string City;
        public string Asset;

        public branchListItem() {
            Name = Num = City = Asset = "NULL";
        }

        public branchListItem(string Name, string Num, string City, double Asset) {
            Init(Name, Num, City, Asset);
        }

        public branchListItem(string Name, string Num, string City, string Asset) {
            Init(Name, Num, City, Asset);
        }

        public branchListItem(branchListItem item) {
            Init(item);
        }

        public void Init(string Name, string Num, string City, double Asset) {
            this.Name = Name;
            this.Num = Num;
            this.City = City;
            this.Asset = Asset.ToString();
        }

        public void Init(string Name, string Num, string City, string Asset) {
            this.Name = Name;
            this.Num = Num;
            this.City = City;
            this.Asset = Asset;
        }

        public void Init(branchListItem item) {
            this.Name = item.Name;
            this.Num = item.Num;
            this.City = item.City;
            this.Asset = item.Asset;
        }
    }

    public class MySqlManager {
        public MySqlConnection con = new MySqlConnection();
        public MySqlManager(string ConStr, TextBlock TxtAlert) {
            con.ConnectionString = ConStr;
            try {
                con.Open();
                TxtAlert.Text = "OK";
            }
            catch(MySqlException ex) {
                TxtAlert.Text = ex.Message;
            }
        }
    }

    public sealed partial class branch : Page {
        private MySqlConnection con = new MySqlConnection();
        private ObservableCollection<branchListItem> data;
        private branchListItem currItem = new branchListItem();
        // head of the table
        private branchListItem colHead = new branchListItem("NAME", "NUMBER", "CITY", "ASSET");
        //public List<branchListItem> list = new List<branchListItem>();
        
        public branch() {
            this.InitializeComponent();

            // build connection with mysql
            string ConStr = "SslMode=None;server=127.0.0.1;userid=root;password=;database=bank;";
            con.ConnectionString = ConStr;
            try {
                con.Open();
                TxtAlert.Text = "OK";
            }
            catch (MySqlException ex) {
                TxtAlert.Text = ex.Message;
            }

            // bind data with listView
            data = new ObservableCollection<branchListItem>();
            ListView.DataContext = data;

            // head of the table
            data.Add(colHead);
        }



        private void BtnClickAdd(object sender, RoutedEventArgs e) {
            string sqlStr = "insert into branch values (\"" + TxtName.Text + "\", \"";
            sqlStr = sqlStr + TxtNum.Text + "\", \"" + TxtCity.Text + "\", " + TxtAsset.Text + ");";

            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            try {
                cmd.ExecuteNonQuery();
                TxtAlert.Text = "Success to add an item! Click Query button to refresh.";
            }
            catch (MySqlException ex) {
                TxtAlert.Text = "Fail to add a new item!\nError message: ";
                TxtAlert.Text += ex.Message;
            }
        }

        private void BtnClickDel(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.Name == null) TxtAlert.Text = "Please select an item!";
            else {
                string sqlStr = "delete from branch where branchName = \"" + currItem.Name + "\";";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to delete an item! Click Query button to refresh.";
                }
                catch (MySqlException ex) {
                    TxtAlert.Text = "Fail to delete an item!\nError message: ";
                    TxtAlert.Text += ex.Message;
                }
            }
        }
        
        private void BtnClickUpd(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.Name == null) TxtAlert.Text = "Please select an item!";
            else {
                string sqlStr = "update branch set bankNum = \"" + TxtNum.Text + "\", branchCity = \"";
                sqlStr += TxtCity.Text + "\", branchAsset = " + TxtAsset.Text;
                sqlStr += " where branchName = \"" + currItem.Name + "\";";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to update an item! Click Query button to refresh.";
                }catch(MySqlException ex) {
                    TxtAlert.Text = "Fail to update an item!\nError message: ";
                    TxtAlert.Text += ex.Message;
                }
                //TxtAlert.Text = sqlStr;
            }
        }

        private void BtnClickQue(object sender, RoutedEventArgs e) {
            data.Clear();
            data.Add(colHead);
            string sqlStr = "select * from branch;";
            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader();
                branchListItem item = new branchListItem();
                while (reader.Read()) {
                    item.Name = reader.GetString("branchName");
                    item.Num = reader.GetString("bankNum");
                    item.City = reader.GetString("branchCity");
                    item.Asset = reader.GetString("branchAsset").ToString();
                    data.Add(new branchListItem(item));
                }
                TxtAlert.Text = "query success!";
            }
            catch(MySqlException ex) {
                TxtAlert.Text = "Fail to execute sql command\nError message: ";
                TxtAlert.Text += ex.Message;
            }
            finally {
                reader.Close();
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            currItem.Init((branchListItem)e.ClickedItem);
            if (currItem == null) return;
            if (currItem.Name == null) return;
            if (currItem.Name == "NAME") TxtAlert.Text = "This is the header of the table.";
            else {
                TxtAlert.Text = "This is a branch.";
                TxtName.Text = currItem.Name;
                TxtNum.Text = currItem.Num;
                TxtCity.Text = currItem.City;
                TxtAsset.Text = currItem.Asset;
            }
        }
    }
}