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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace newBank {
    public class clientListItem {
        public string cltID, cltName, cltPhone, cltAddr;
        public string ctcName, ctcPhone, ctcEmail, relation;

        public clientListItem() {
            cltID = cltName = cltPhone = cltAddr = null;
            ctcName = ctcPhone = ctcEmail = relation = null;
        }

        public clientListItem(string cltID, string cltName, string cltPhone, string cltAddr,
                            string ctcName, string ctcPhone, string ctcEmail, string relation) {
            Init(cltID, cltName, cltPhone, cltAddr, ctcName, ctcPhone, ctcEmail, relation);
        }

        public clientListItem(clientListItem item) {
            Init(item);
        }

        public void Init(   string cltID, string cltName, string cltPhone, string cltAddr,
                            string ctcName, string ctcPhone, string ctcEmail, string relation) {
            this.cltID = cltID;
            this.cltName = cltName;
            this.cltPhone = cltPhone;
            this.cltAddr = cltAddr;
            this.ctcName = ctcName;
            this.ctcPhone = ctcPhone;
            this.ctcEmail = ctcEmail;
            this.relation = relation;
        }

        public void Init(clientListItem item) {
            cltID = item.cltID;
            cltName = item.cltName;
            cltPhone = item.cltPhone;
            cltAddr = item.cltAddr;
            ctcName = item.ctcName;
            ctcPhone = item.ctcPhone;
            ctcEmail = item.ctcEmail;
            relation = item.relation;
        }
    }


    public sealed partial class Client : Page {
        private MySqlConnection con = new MySqlConnection();
        private ObservableCollection<clientListItem> data;
        private clientListItem currItem = new clientListItem();

        // head of the table
        private clientListItem colHead = new clientListItem("CLIENT ID", "CLIENT NAME", "CLIENT PHONE", "CLIENT ADDRESS",
            "CONTACT NAME", "CONTACT PHONE", "CONTACT EMAIL", "CONTACT RELATION");

        public Client() {
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
            data = new ObservableCollection<clientListItem>();
            ListView.DataContext = data;

            // head of the table
            data.Add(colHead);
        }


        private void BtnClickAdd(object sender, RoutedEventArgs e) {
            string sqlStr = "insert into client values ('" + TxtCltID.Text + "', '" + TxtCltName.Text;
            sqlStr += "', '" + TxtCltPhone.Text + "', '" + TxtCltAddr.Text;
            sqlStr += "', '" + TxtCtcName.Text + "', '" + TxtCtcPhone.Text;
            sqlStr += "', '" + TxtCtcEmail.Text + "', '" + TxtRel.Text + "');";

            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            try {
                cmd.ExecuteNonQuery();
                TxtAlert.Text = "Success to add a client! Click Query button to refresh.";
            }
            catch (MySqlException ex) {
                TxtAlert.Text = "Fail to add a new client!\nError message: ";
                TxtAlert.Text += ex.Message;
            }
        }

        private void BtnClickDel(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.cltID == null) TxtAlert.Text = "Please select a client!";
            else {
                string sqlStr = "delete from client where clientID = '" + currItem.cltID + "';";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to delete a client! Click Query button to refresh.";
                }
                catch (MySqlException ex) {
                    TxtAlert.Text = "Fail to delete a client!\nError message: ";
                    TxtAlert.Text += ex.Message;
                }
            }
        }

        private void BtnClickUpd(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.cltID == null) TxtAlert.Text = "Please select an client!";
            else {
                string sqlStr = "update client set clientName = '" + TxtCltName.Text + "', clientPhone = '";
                sqlStr += TxtCltPhone.Text + "', clientAddr = '" + TxtCltAddr.Text + "', contactName = '";
                sqlStr += TxtCtcName.Text + "', contactPhone = '" + TxtCtcPhone.Text + "', contactEmail = '";
                sqlStr += TxtCtcEmail.Text + "', relationWithContact = '" + TxtRel.Text + "' ";
                sqlStr += "where clientID = '" + currItem.cltID + "';";

                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to update a client! Click Query button to refresh.";
                }
                catch (MySqlException ex) {
                    TxtAlert.Text = "Fail to update a client!\nError message: ";
                    TxtAlert.Text += ex.Message;
                }
            }
        }

        private void BtnClickQue(object sender, RoutedEventArgs e) {
            data.Clear();
            data.Add(colHead);
            string sqlStr = "select * from client;";
            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader();
                clientListItem item = new clientListItem();
                while (reader.Read()) {
                    item.cltID = reader.GetString("clientID");
                    item.cltName = reader.GetString("clientName");
                    item.cltPhone = reader.GetString("clientPhone");
                    item.cltAddr = reader.GetString("clientAddr");


                    item.ctcName = reader.GetString("contactName");
                    item.ctcPhone = reader.GetString("contactPhone");
                    item.ctcEmail = reader.GetString("contactEmail");
                    item.relation = reader.GetString("relationWithContact");
                    data.Add(new clientListItem(item));
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
            currItem.Init((clientListItem)e.ClickedItem);
            if (currItem == null) return;
            if (currItem.cltID == null) return;
            if (currItem.cltID == "CLIENT ID") TxtAlert.Text = "This is the header of the table.";
            else {
                TxtAlert.Text = "This is a client.";
                TxtCltID.Text = currItem.cltID;
                TxtCltName.Text = currItem.cltName;
                TxtCltPhone.Text = currItem.cltPhone;
                TxtCltAddr.Text = currItem.cltAddr;

                TxtCtcName.Text = currItem.ctcName;
                TxtCtcPhone.Text = currItem.ctcPhone;
                TxtCtcEmail.Text = currItem.ctcEmail;
                TxtRel.Text = currItem.relation;
            }
        }
    }
}