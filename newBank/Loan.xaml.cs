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
    public class LoanListItem {
        public string loanNum;
        public string totalPay;
        public string BName;
        public string CID;

        public LoanListItem() {
            loanNum = totalPay = null;
            BName = CID = null;
        }

        public LoanListItem(string loanNum, string totalPay, string BName, string CID) {
            Init(loanNum, totalPay, BName, CID);
        }

        public LoanListItem(LoanListItem item) {
            Init(item);
        }

        public void Init(string loanNum, string totalPay, string BName, string CID) {
            this.loanNum = loanNum;
            this.totalPay = totalPay;
            this.BName = BName;
            this.CID = CID;
        }

        public void Init(LoanListItem item) {
            loanNum = item.loanNum;
            totalPay = item.totalPay;
            BName = item.BName;
            CID = item.CID;
        }
    }

    public class SLoanListItem {
        public string LNum, SLNum, pay, TotalPay, date;

        public SLoanListItem() {
            LNum = SLNum = pay = TotalPay = date = null;
        }

        public SLoanListItem(string LNum, string SLNum, string pay, string TotalPay, string date) {
            Init(LNum, SLNum, pay, TotalPay, date);
        }

        public SLoanListItem(SLoanListItem item) {
            Init(item);
        }

        public void Init(string LNum, string SLNum, string pay, string TotalPay, string date) {
            this.LNum = LNum;
            this.SLNum = SLNum;
            this.pay = pay;
            this.TotalPay = TotalPay;
            this.date = date;
        }

        public void Init(SLoanListItem item) {
            LNum = item.LNum;
            SLNum = item.SLNum;
            pay = item.pay;
            TotalPay = item.TotalPay;
            date = item.date;
        }
    }

    public sealed partial class Loan : Page {
        private MySqlConnection con = new MySqlConnection();
        private ObservableCollection<LoanListItem> data;
        private LoanListItem currItem = new LoanListItem();
        private int SLCount = 5;

        // head of the table
        private LoanListItem colHead = new LoanListItem("LOAN NUMBER", "TOTAL PAYMENT", "BRANCH NAME", "CLIENT ID");

        private ObservableCollection<SLoanListItem> _data;
        private SLoanListItem _currItem = new SLoanListItem();

        // head of the table
        private SLoanListItem _colHead = new SLoanListItem("LOAN NUMBER", "SINGLE LOAN NUBMBER", "PAYMENT", "TOTAL PAYMENT", "DATE");


        public Loan() {
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
            data = new ObservableCollection<LoanListItem>();
            LoanListView.DataContext = data;

            _data = new ObservableCollection<SLoanListItem>();
            SLoanListView.DataContext = _data;

            // head of the table
            data.Add(colHead);
            _data.Add(_colHead);
        }


        private void BtnClickApply(object sender, RoutedEventArgs e) {
            string sqlStr = "call applyLoan('" + TxtLNum.Text + "', " + TxtTPay.Text + ", '" + TxtBName.Text;
            sqlStr += "', '" + TxtCID.Text + "');";

            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            try {
                cmd.ExecuteNonQuery();
                TxtAlert.Text = "Success to apply a loan! Click Query button to refresh.";
            }
            catch (MySqlException ex) {
                TxtAlert.Text = "Fail to apply!\nError message: ";
                TxtAlert.Text += ex.Message;
            }
        }


        private void BtnClickLoans(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.loanNum == null) TxtAlert.Text = "Please choose an item!";
            else {
                string sqlStr = "call offerLoan('" + currItem.loanNum + "', 'SL" + SLCount + "', " + TxtTPay.Text + ");";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to loan! Click Query button to refresh.";
                }
                catch (MySqlException ex) {
                    TxtAlert.Text = "Fail to loan!\nError message: ";
                    TxtAlert.Text += ex.Message;
                }
                SLCount++;
            }
        }

        private void BtnClickDel(object sender, RoutedEventArgs e) {
            if (currItem == null || currItem.loanNum == null) TxtAlert.Text = "Please choose an item!";
            else {
                //string sqlStr = "call offerLoan('" + currItem.loanNum + "', 'SL" + SLCount + "');";
                string sqlStr = "call delLoan('" + currItem.loanNum + "');";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                try {
                    cmd.ExecuteNonQuery();
                    TxtAlert.Text = "Success to delete a loan! Click Query button to refresh.";
                }
                catch (MySqlException ex) {
                    //TxtAlert.Text = "Fail to loan!\nError message: ";
                    TxtAlert.Text = "Loaning! Not allowed to delete.";
                    //TxtAlert.Text += ex.Message;
                }
                SLCount++;
            }

        }

        private void BtnClickQue(object sender, RoutedEventArgs e) {
            data.Clear();
            data.Add(colHead);
            string sqlStr = "call showLoan();";
            MySqlCommand cmd = new MySqlCommand(sqlStr, con);
            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader();
                LoanListItem item = new LoanListItem();
                while (reader.Read()) {
                    item.loanNum = reader.GetString(0);
                    item.totalPay = reader.GetString(1);
                    item.BName = reader.GetString(2);
                    item.CID = reader.GetString(3);
                    
                    data.Add(new LoanListItem(item));
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


        private void LoanListView_ItemClick(object sender, ItemClickEventArgs e) {
            currItem.Init((LoanListItem)e.ClickedItem);
            if (currItem == null) return;
            if (currItem.loanNum == null) return;
            if (currItem.loanNum == "LOAN NUMBER") TxtAlert.Text = "This is the head of table loan.";
            else {
                _data.Clear();
                _data.Add(_colHead);
                string sqlStr = "call showSLoan('" + currItem.loanNum + "');";
                MySqlCommand cmd = new MySqlCommand(sqlStr, con);
                MySqlDataReader reader = null;
                double totalpay = 0, singlepay = 0, sum = 0;
                Double.TryParse(currItem.totalPay, out totalpay);
                try {
                    reader = cmd.ExecuteReader();
                    SLoanListItem item = new SLoanListItem();
                    while (reader.Read()) {
                        item.LNum = reader.GetString(0);
                        item.SLNum = reader.GetString(1);
                        item.pay = reader.GetString(2);
                        item.TotalPay = reader.GetString(3);
                        item.date = reader.GetDateTime(4).ToLongDateString();

                        _data.Add(new SLoanListItem(item));
                        Double.TryParse(item.TotalPay, out totalpay);
                        Double.TryParse(item.pay, out singlepay);
                        sum += singlepay;
                    }
                    if (Math.Abs(sum - totalpay) < 0.01) TxtAlert.Text = "Done!";
                    else if (Math.Abs(sum - 0) < 0.01) TxtAlert.Text = "Not start yet!";
                    else TxtAlert.Text = "Loaning...";
                    _TxtAlert.Text = "Query success!";
                }
                catch (MySqlException ex) {
                    _TxtAlert.Text = "Fail to execute sql command!\nError message: ";
                    _TxtAlert.Text += ex.Message;
                }
                finally {
                    reader.Close();
                }

                TxtLNum.Text = currItem.loanNum;
                TxtTPay.Text = currItem.totalPay;
                TxtBName.Text = currItem.BName;
                TxtCID.Text = currItem.CID;
            }
        }

        private void SLoanListView_ItemClick(object sender, ItemClickEventArgs e) {
            _currItem.Init((SLoanListItem)e.ClickedItem);

        }
    }
}