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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class staffListItem {
        public string ID;
        public string manaID;
        public string Name;
        public string Phone;
        public string Addr;
        public string WorkDate;

        public staffListItem() {
            ID = manaID = Name = Phone = Addr = WorkDate = "NULL";
        }

        public staffListItem(string ID, string manaID, string Name, string Phone, string Addr, string WorkDate) {
            Init(ID, manaID, Name, Phone, Addr, WorkDate);
        }

        public staffListItem(staffListItem item) {
            Init(item);
        }

        public void Init(string ID, string manaID, string Name, string Phone, string Addr, string WorkDate) {
            this.ID = ID;
            this.manaID = manaID;
            this.Name = Name;
            this.Phone = Phone;
            this.Addr = Addr;
            this.WorkDate = WorkDate;
        }

        public void Init(staffListItem item) {
            this.ID = item.ID;
            this.manaID = item.manaID;
            this.Name = item.Name;
            this.Phone = item.Phone;
            this.Addr = item.Addr;
            this.WorkDate = item.WorkDate;
        }

    }

    public class RAIIReader {
        private MySqlDataReader reader;
        public RAIIReader(MySqlCommand cmd) {
            reader = cmd.ExecuteReader();
        }
        ~RAIIReader() {
            reader.Close();
        }
        public string GetString(string str) {
            return reader.GetString(str);
        }
        public DateTime GetDateTime(string str) {
            return reader.GetDateTime(str);
        }
        public bool Read() {
            return reader.Read();
        }
    }

    public sealed partial class Staff : Page {
        private MySqlConnection con = new MySqlConnection();
        private ObservableCollection<staffListItem> data;
        private staffListItem currItem = new staffListItem();

        // head of the table
        private staffListItem colHead = new staffListItem("STAFF ID", "MANAGER ID", "NAME", "PHONE NUM", "ADDRESS", "WORK DATE");

        public Staff() {
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
            data = new ObservableCollection<staffListItem>();
            ListView.DataContext = data;

            // head of the table
            data.Add(colHead);
        }

        private void BtnClickAddStaff(object sender, RoutedEventArgs e) {
            if (TxtManaID.Text == "")
                TxtAlert.Text = "Fail to add a new staff!\nError message: Must choose a manager when add a staff.";
            else {
                string sqlStr = "insert into staff values ('" + TxtID.Text + "', '" + TxtManaID.Text + "', '";
                sqlStr += TxtName.Text + "', '" + TxtPhone.Text + "', '" + TxtAddr.Text + "', ";
                sqlStr += "str_to_date('" + TxtWorkDate.Text + "', '%m/%d/%Y'));";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);

                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to add a new staff! Click Query button to refresh.";
                }
                catch (MySqlException ex) {
                    TxtAlert.Text = "Fail to add a new staff!\nError message: ";
                    TxtAlert.Text += ex.Message;
                }
            }
        }


        private void BtnClickAddMana(object sender, RoutedEventArgs e) {
            string sqlStaffStr = "insert into staff values ('" + TxtID.Text + "', null, '";
            sqlStaffStr += TxtName.Text + "', '" + TxtPhone.Text + "', '" + TxtAddr.Text + "', ";
            sqlStaffStr += "str_to_date('" + TxtWorkDate.Text + "', '%m/%d/%Y'));";
            MySqlCommand staffCmd = new MySqlCommand(sqlStaffStr, con);

            string sqlManaStr = "insert into depManager values ('" + TxtID.Text + "', '";
            sqlManaStr += TxtName.Text + "', '" + TxtPhone.Text + "', '" + TxtAddr.Text + "', ";
            sqlManaStr += "str_to_date('" + TxtWorkDate.Text + "', '%m/%d/%Y'));";
            MySqlCommand manaCmd = new MySqlCommand(sqlManaStr, con);
            try {
                staffCmd.ExecuteNonQuery();
                manaCmd.ExecuteNonQuery();
                TxtAlert.Text = "Success to add a manager! Click Query button to update.";
            }
            catch (MySqlException ex) {
                TxtAlert.Text = "Fail to add a manager!\nError message: ";
                TxtAlert.Text += ex.Message;
            }
        }

        private void BtnClickDel(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.Name == null) TxtAlert.Text = "Please select an item!";
            else {
                if(currItem.manaID == "NULL") {
                    // delete an manager 
                    string sqlManaStr = "delete from depManager where staffID = '" + currItem.ID + "';";
                    MySqlCommand manaCmd = new MySqlCommand(sqlManaStr, con);

                    string sqlStaffStr = "delete from staff where staffID = '" + currItem.ID + "';";
                    MySqlCommand staffCmd = new MySqlCommand(sqlStaffStr, con);

                    try {
                        manaCmd.ExecuteNonQuery();
                        staffCmd.ExecuteNonQuery();
                        TxtAlert.Text = "Success to delete a manager! Click Query button to refresh.";
                    }
                    catch (MySqlException ex) {
                        TxtAlert.Text = "Fail to delete a manager!\nError message: ";
                        TxtAlert.Text += ex.Message;
                    }
                }
                else {
                    // delete a staff
                    string sqlStaffStr = "delete from staff where staffID = '" + currItem.ID + "';";
                    MySqlCommand staffCmd = new MySqlCommand(sqlStaffStr, con);

                    try {
                        staffCmd.ExecuteNonQuery();
                        TxtAlert.Text = "Success to delete a staff! Click Query button to refresh.";
                    }
                    catch (MySqlException ex) {
                        TxtAlert.Text = "Fail to delete a staff!\nError message: ";
                        TxtAlert.Text += ex.Message;
                    }
                }
            }
        }

        private void BtnClickUpd(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.Name == null) TxtAlert.Text = "Please select an item!";
            else {
                if(currItem.manaID == "NULL") {
                    // update a manager 
                    string sqlStaffStr = "update staff set staffName = '" + TxtName.Text + "', staffPhone = '";
                    sqlStaffStr += TxtPhone.Text + "', staffAddr = '" + TxtAddr.Text + "', startWorkDate = ";
                    sqlStaffStr += "str_to_date('" + TxtWorkDate.Text + "', '%m/%d/%Y') ";
                    sqlStaffStr += "where staffID = '" + currItem.ID + "';";
                    MySqlCommand staffCmd = new MySqlCommand(sqlStaffStr, con);

                    string sqlManaStr = "update depManager set staffName = '" + TxtName.Text + "', staffPhone = '";
                    sqlStaffStr += TxtPhone.Text + "', staffAddr = '" + TxtAddr.Text + "', startWorkDate = ";
                    sqlStaffStr += "str_to_date('" + TxtWorkDate.Text + "', '%m/%d/%Y') ";
                    sqlStaffStr += "where staffID = '" + currItem.ID + "';";
                    MySqlCommand manaCmd = new MySqlCommand(sqlManaStr, con);

                    try {
                        staffCmd.ExecuteNonQuery();
                        manaCmd.ExecuteNonQuery();
                        TxtAlert.Text = "Success to update a manager! Click Query button to refresh.";
                    }
                    catch (MySqlException ex) {
                        TxtAlert.Text = "Fail to update an manager!\nError message: ";
                        TxtAlert.Text += ex.Message;
                    }
                }
                else {
                    // update a staff
                    string sqlStaffStr = "update staff set dep_staffID = '" + TxtManaID.Text;
                    sqlStaffStr += "', staffName = '" + TxtName.Text + "', staffPhone = '";
                    sqlStaffStr += TxtPhone.Text + "', staffAddr = '" + TxtAddr.Text + "', startWorkDate = ";
                    sqlStaffStr += "str_to_date('" + TxtWorkDate.Text + "', '%m/%d/%Y') ";
                    sqlStaffStr += "where staffID = '" + currItem.ID + "';";
                    MySqlCommand staffCmd = new MySqlCommand(sqlStaffStr, con);

                    try {
                        staffCmd.ExecuteNonQuery();
                        TxtAlert.Text = "Success to update a staff! Click Query button to refresh.";
                    }
                    catch (MySqlException ex) {
                        TxtAlert.Text = "Fail to update a staff!\nError message: ";
                        TxtAlert.Text += ex.Message;
                    }
                }
            }
        }

        private void BtnClickQue(object sender, RoutedEventArgs e) {
            data.Clear();
            data.Add(colHead);
            string sqlStr = "select * from staff;";
            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            MySqlDataReader reader = null;
            try {
                //MySqlDataReader reader = cmd.ExecuteReader();
                staffListItem item = new staffListItem();
                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    if (reader.IsDBNull(0)) item.ID = "NULL";
                    else item.ID = reader.GetString("staffID");

                    if (reader.IsDBNull(1)) item.manaID = "NULL";
                    else item.manaID = reader.GetString("dep_staffID");

                    if (reader.IsDBNull(2)) item.Name = "NULL";
                    else item.Name = reader.GetString("staffName");

                    if (reader.IsDBNull(3)) item.Phone = "NULL";
                    else item.Phone = reader.GetString("staffPhone");

                    if (reader.IsDBNull(4)) item.Addr = "NULL";
                    else item.Addr = reader.GetString("staffAddr");

                    if (reader.IsDBNull(5)) item.WorkDate = "NULL";
                    else item.WorkDate = reader.GetDateTime("startWorkDate").ToLongDateString();
                    data.Add(new staffListItem(item));
                }
                TxtAlert.Text = "query success!";
            }
            catch (Exception ex) {
                TxtAlert.Text = "Fail to execute sql command\nError message: ";
                TxtAlert.Text += ex.Message;
            }
            finally {
                if (reader != null) reader.Close();
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            currItem.Init((staffListItem)e.ClickedItem);
            if (currItem == null) return;
            if (currItem.manaID == null) return;
            if (currItem.manaID == "MANAGER ID") TxtAlert.Text = "This is the header of the table.";
            else {
                if (currItem.manaID == "NULL") TxtAlert.Text = "This is a manager.";
                else TxtAlert.Text = "This is a staff.";

                TxtID.Text = currItem.ID;
                TxtManaID.Text = currItem.manaID;
                TxtName.Text = currItem.Name;
                TxtPhone.Text = currItem.Phone;
                TxtAddr.Text = currItem.Addr;
                TxtWorkDate.Text = "mm/dd/YYYY";
            }
        }
    }
}