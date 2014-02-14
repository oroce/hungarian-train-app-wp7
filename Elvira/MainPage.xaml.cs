using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.Phone.Shell;

namespace Elvira
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        const string ELVIRA_URL = "http://api.oroszi.net/elvira";
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string from = txtFrom.Text.Trim();
            string to = txtTo.Text.Trim();
            DateTime? when = null;
            string fromTime = null;
            if (String.IsNullOrEmpty(from) || String.IsNullOrEmpty(to)) {
                MessageBox.Show("A honnan és hová mezőket kötelező kitölteni!");
                return;
            }
            if (datePicker.Value != null)
            {
                when = (DateTime)datePicker.Value;

                if (timePicker.Value != null)
                {
                    DateTime currTime = (DateTime)timePicker.Value;
                    fromTime = String.Format("{0}:{1}", currTime.Hour, currTime.Minute);
                }
            }
            WebClient wcElvira = new WebClient();
            string uri = string.Format("{0}?from={1}&to={2}&date={3}&fromtime={4}&content-type=application/json", ELVIRA_URL, from, to, when == null ? "" : ((DateTime)when).ToString("yyyy.MM.dd"), fromTime == null ? "" : fromTime);
            Debug.WriteLine(uri);
            Uri elviraUri = new Uri( uri );
        
            wcElvira.OpenReadCompleted +=new OpenReadCompletedEventHandler(wcElvira_OpenReadCompleted);
            wcElvira.OpenReadAsync( elviraUri );
            

        }

        void wcElvira_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine(e.Error);
                MessageBox.Show("Hiba történt!");
                return;

            }
            try
            {
                DataContractJsonSerializer dataContractJsonSerializer =
                            new DataContractJsonSerializer(typeof(TimetableResponse));

                TimetableResponse timetableResp =
            (TimetableResponse)dataContractJsonSerializer.ReadObject(e.Result);
                if (!String.IsNullOrEmpty(timetableResp.errordesc))
                {
                    MessageBox.Show(timetableResp.errormsg);
                    return;
                }
                PhoneApplicationService.Current.State["timetable"] = timetableResp;

                NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.Relative));
            }
            catch (Exception ex) {
                MessageBox.Show("Hiba történt!");
                return;
            }
        }
    }

    public class Detail
    {
        public string starttime_real { get; set; }
        public string starttime { get; set; }
        public string platform { get; set; }
        public string traininfo { get; set; }
        public string start { get; set; }
    }

    public class Ticket
    {
        public string full_url_m { get; set; }
        public string m_url { get; set; }
        public string url { get; set; }
        public string full_url { get; set; }
    }

    public class Timetable
    {
        public string class_name { get; set; }
        public string cost1st { get; set; }
        public string destinationtime { get; set; }
        public string reservation { get; set; }
        public string distance { get; set; }
        public string change { get; set; }
        public string destination { get; set; }
        public List<Detail> details { get; set; }
        public string totaltime { get; set; }
        public string starttime { get; set; }
        public string cost2nd { get; set; }
        public Ticket ticket { get; set; }
        public string start { get; set; }
    }

    public class TimetableResponse
    {
        public string date { get; set; }
        public string route { get; set; }
        public List<Timetable> timetable { get; set; }
        public string errordesc { get; set; }
        public string errormsg { get; set; }
        public int error { get; set; }
    }
}