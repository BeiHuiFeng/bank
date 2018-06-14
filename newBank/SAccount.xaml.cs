using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace newBank {
    public class SAccListItem {
        public string SAccNum, BName, CID, LVDate;
        public string balance, applyDate, interest, currency;

        public SAccListItem () {
            SAccNum = BName = CID = LVDate = null;
            balance = applyDate = interest = currency = null;
        }

        public SAccListItem(string SAccNum, string BName, string CID, string LVDate,
            string balance, string applyDate, string interest, string currency) {
            this.SAccNum = SAccNum;
            this.BName = BName;
            this.CID = CID;
            this.LVDate = LVDate;
            this.balance = balance;
            this.applyDate = applyDate;
            this.interest = interest;
            this.currency = currency;
        }

        public SAccListItem(SAccListItem item) {
            Init(item);
        }

        public void Init(string SAccNum, string BName, string CID, string LVDate,
            string balance, string applyDate, string interest, string currency) {
            this.SAccNum = SAccNum;
            this.BName = BName;
            this.CID = CID;
            this.LVDate = LVDate;
            this.balance = balance;
            this.applyDate = applyDate;
            this.interest = interest;
            this.currency = currency;
        }

        public void Init(SAccListItem item) {
            this.SAccNum = item.SAccNum;
            this.BName = item.BName;
            this.CID = item.CID;
            this.LVDate = item.LVDate;
            this.balance = item.balance;
            this.applyDate = item.applyDate;
            this.interest = item.interest;
            this.currency = item.currency;
        }
    }

    public sealed partial class SAccount : Page {
        private MySqlConnection con = new MySqlConnection();
        private ObservableCollection<SAccListItem> data;
        private SAccListItem currItem = new SAccListItem();
        
        // head of the table
        private SAccListItem colHead = new SAccListItem("ACC NUMBER", "BRANCH NAME", "CLIENT ID", "LAST VISIT DATE",
            "BALANCE", "APPLY DATE", "INTEREST RATE", "CURRENCY");

        public SAccount() {
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
            data = new ObservableCollection<SAccListItem>();
            ListView.DataContext = data;

            // head of the table
            data.Add(colHead);
        }

        private void BtnClickApply(object sender, RoutedEventArgs e) {
            string sqlStr = "call applySAcc('" + TxtCID.Text + "', '" + TxtBName.Text + "', '";
            sqlStr += TxtSANum.Text + "');";

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
            if (currItem == null || currItem.SAccNum == null) TxtAlert.Text = "Please select a client!";
            else {
                string sqlStr = "call deleSAcc('" + TxtCID.Text + "', '" + TxtBName.Text + "', '";
                sqlStr += TxtSANum.Text + "');";
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
            if (currItem == null || currItem.SAccNum == null) TxtAlert.Text = "Please select a client!";
            else {
                //string sqlStr = "call updateSAcc('" + TxtCID.Text + "', '" + TxtBName.Text + "', '";
                //sqlStr += TxtSANum.Text + "');";
                string sqlStr = "call updateSAcc('" + currItem.SAccNum + "', '" + currItem.BName + "', '";
                sqlStr += currItem.CID + "', '" + TxtBName.Text + "', '" + TxtCID.Text + "', str_to_date('";
                sqlStr += TxtLVDate.Text + "', '%m/%d/%Y'), " + TxtBal.Text + ", str_to_date('";
                sqlStr += TxtADate.Text + "', '%m/%d/%Y'), " + TxtItrt.Text + ", '" + TxtCrrc.Text + "');";
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

        private void BtnClickQue(object sender, RoutedEventArgs e) {
            data.Clear();
            data.Add(colHead);
            string sqlStr = "call showSAcc;";
            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader();
                SAccListItem item = new SAccListItem();
                while (reader.Read()) {
                    item.SAccNum = reader.GetString(0);
                    item.BName = reader.GetString(1);
                    item.CID = reader.GetString(2);
                    item.LVDate = reader.GetDateTime(3).ToLongDateString();
                    
                    item.balance = reader.GetString(4);
                    item.applyDate = reader.GetDateTime(5).ToLongDateString();
                    item.interest = reader.GetString(6);
                    item.currency = reader.GetString(7);
                    data.Add(new SAccListItem(item));
                }
                TxtAlert.Text = "Query success!";
            }
            catch (MySqlException ex) {
                TxtAlert.Text = "Fail to execute sql command!\nError message: ";
                TxtAlert.Text += ex.Message;
            }
            finally {
                reader.Close();
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            currItem.Init((SAccListItem)e.ClickedItem);
            if (currItem == null) return;
            if (currItem.SAccNum == null) return;
            if(currItem.SAccNum == "ACC NUMBER") TxtAlert.Text = "This is the header of the table.";
            else {
                TxtAlert.Text = "This is a saving account.";
                TxtSANum.Text = currItem.SAccNum;
                TxtBName.Text = currItem.BName;
                TxtCID.Text = currItem.CID;
                TxtLVDate.Text = "mm/dd/YYYY";

                TxtBal.Text = currItem.balance;
                TxtADate.Text = "mm/dd/YYYY";
                TxtItrt.Text = currItem.interest;
                TxtCrrc.Text = currItem.currency;
            }
        }
    }
}