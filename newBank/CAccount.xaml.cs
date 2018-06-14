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
    public class CAccListItem {
        public string SAccNum, BName, CID, LVDate;
        public string balance, applyDate, ODraft;

        public CAccListItem() {
            SAccNum = BName = CID = LVDate = null;
            balance = applyDate = ODraft = null;
        }

        public CAccListItem(string SAccNum, string BName, string CID, string LVDate,
            string balance, string applyDate, string ODraft) {
            this.SAccNum = SAccNum;
            this.BName = BName;
            this.CID = CID;
            this.LVDate = LVDate;
            this.balance = balance;
            this.applyDate = applyDate;
            this.ODraft = ODraft;
        }

        public CAccListItem(CAccListItem item) {
            Init(item);
        }

        public void Init(string SAccNum, string BName, string CID, string LVDate,
            string balance, string applyDate, string ODraft) {
            this.SAccNum = SAccNum;
            this.BName = BName;
            this.CID = CID;
            this.LVDate = LVDate;
            this.balance = balance;
            this.applyDate = applyDate;
            this.ODraft = ODraft;
        }

        public void Init(CAccListItem item) {
            SAccNum = item.SAccNum;
            BName = item.BName;
            CID = item.CID;
            LVDate = item.LVDate;
            balance = item.balance;
            applyDate = item.applyDate;
            ODraft = item.ODraft;
        }
    }

    public sealed partial class CAccount : Page {
        private MySqlConnection con = new MySqlConnection();
        private ObservableCollection<CAccListItem> data;
        private CAccListItem currItem = new CAccListItem();

        // head of the table
        private CAccListItem colHead = new CAccListItem("ACC NUMBER", "BRANCH NAME", "CLIENT ID", "LAST VISIT DATE",
            "BALANCE", "APPLY DATE", "OVER DRAFT");

        public CAccount() {
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
            data = new ObservableCollection<CAccListItem>();
            ListView.DataContext = data;

            // head of the table
            data.Add(colHead);

        }

        private void BtnClickApply(object sender, RoutedEventArgs e) {
            string sqlStr = "call applyCAcc('" + TxtCID.Text + "', '" + TxtBName.Text + "', '";
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
                string sqlStr = "call deleCAcc('" + TxtCID.Text + "', '" + TxtBName.Text + "', '";
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
                string sqlStr = "call updateCAcc('" + currItem.SAccNum + "', '" + currItem.BName + "', '";
                sqlStr += currItem.CID + "', '" + TxtBName.Text + "', '" + TxtCID.Text + "', str_to_date('";
                sqlStr += TxtLVDate.Text + "', '%m/%d/%Y'), " + TxtBal.Text + ", str_to_date('";
                sqlStr += TxtADate.Text + "', '%m/%d/%Y'), " + TxtODraft.Text + ");";
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
            string sqlStr = "call showCAcc;";
            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader();
                CAccListItem item = new CAccListItem();
                while (reader.Read()) {
                    item.SAccNum = reader.GetString(0);
                    item.BName = reader.GetString(1);
                    item.CID = reader.GetString(2);
                    item.LVDate = reader.GetDateTime(3).ToLongDateString();

                    item.balance = reader.GetString(4);
                    item.applyDate = reader.GetDateTime(5).ToLongDateString();
                    item.ODraft = reader.GetString(6);
                    data.Add(new CAccListItem(item));
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
            currItem.Init((CAccListItem)e.ClickedItem);
            if (currItem == null) return;
            if (currItem.SAccNum == null) return;
            if (currItem.SAccNum == "ACC NUMBER") TxtAlert.Text = "This is the header of the table.";
            else {
                TxtAlert.Text = "This is a checking account.";
                TxtSANum.Text = currItem.SAccNum;
                TxtBName.Text = currItem.BName;
                TxtCID.Text = currItem.CID;
                TxtLVDate.Text = "mm/dd/YYYY";

                TxtBal.Text = currItem.balance;
                TxtADate.Text = "mm/dd/YYYY";
                TxtODraft.Text = currItem.ODraft;
            }
        }
    }
}