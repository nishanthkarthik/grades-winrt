using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GradeSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GradePage : Page
    {

        private CookieContainer _cookie;

        public GradePage()
        {
            InitializeComponent();
            Loaded += GradePage_Loaded;
        }

        async void GradePage_Loaded(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage gradeResponse = await RetrieveGrade();
            CookieCollection cookie = _cookie.GetCookies(new Uri("http://www.iitm.ac.in", UriKind.Absolute));
            string gradeSrc = await gradeResponse.Content.ReadAsStringAsync();

        }

        private async Task<HttpResponseMessage> RetrieveGrade()
        {
            HttpClient httpClient = new HttpClient(new HttpClientHandler()
            {
                CookieContainer = _cookie,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });
            httpClient.DefaultRequestHeaders.Referrer =
                new Uri("https://www.iitm.ac.in/viewgrades/studentauth/studpass.php", UriKind.Absolute);
            httpClient.BaseAddress = new Uri("https://www.iitm.ac.in");
            httpClient.DefaultRequestHeaders.Host = "www.iitm.ac.in";
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            httpClient.DefaultRequestHeaders.Add("DNT", "1");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            httpClient.DefaultRequestHeaders.Add("Cookie", _cookie.GetCookies(new Uri("https://www.iitm.ac.in", UriKind.Absolute))["PHPSESSID"].ToString());
            return await httpClient.GetAsync("https://www.iitm.ac.in/viewgrades/studentauth/studopts2.php");

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HttpResponsePack httpResponse = e.Parameter as HttpResponsePack;
            if (httpResponse != null)
                _cookie = httpResponse.Cookie;
        }


    }
}
