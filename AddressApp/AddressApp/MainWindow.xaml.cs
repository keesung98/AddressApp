using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Xml;

class Address
{
    public string zipNo { get; set; }
    public string lnmAdres { get; set; }
    public string rnAdres { get; set; }

    private static List<Address> instance;

    public static List<Address> GetInstance()
    {
        if (instance == null)
            instance = new List<Address>();

        return instance;
    }
}

namespace AddressApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static HttpClient client = new HttpClient();

        static string Search(string srchwrd)
        {
            string url = "http://openapi.epost.go.kr/postal/retrieveNewAdressAreaCdSearchAllService/retrieveNewAdressAreaCdSearchAllService/getNewAddressListAreaCdSearchAll";
            url += "?ServiceKey=" + "서비스키"; // Service Key
            url += $"&srchwrd={srchwrd}";
            url += "&countPerPage=50";
            url += "&countPage=1";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();
            }
            return results;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                string result = Search(txtSearch.Text);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList xmlNodeList=doc.GetElementsByTagName("newAddressListAreaCdSearchAll");

                Address.GetInstance().Clear();

                foreach (XmlNode item in xmlNodeList) 
                {
                    string zipNo= $"{item["zipNo"].InnerText}";
                    string lnmAdres = $"{item["lnmAdres"].InnerText}";
                    string rnAdres = $"{item["rnAdres"].InnerText}";

                    Address.GetInstance().Add(new Address() { zipNo = zipNo, lnmAdres = lnmAdres, rnAdres = rnAdres });
                }
                lstView.ItemsSource = null;
                lstView.ItemsSource = Address.GetInstance();
            }
        }
    }
}