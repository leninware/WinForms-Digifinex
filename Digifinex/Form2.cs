using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;


namespace Digifinex
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public void do_postrequest(string s, string url)
        {
            using (var client = new HttpClient())
            {
                var endpoint = new Uri(url + s);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("ACCESS-KEY", key);
                client.DefaultRequestHeaders.Add("ACCESS-TIMESTAMP", do_access_timestamp());
                client.DefaultRequestHeaders.Add("ACCESS-SIGN", do_access_sign(s));

                var result = client.PostAsync(endpoint, null).Result.Content.ReadAsStringAsync().Result;
                MessageBox.Show(result);
            }
        }
        public string do_getrequest(string s, string url)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Add("ACCESS-KEY", key);
            client.DefaultRequestHeaders.Add("ACCESS-TIMESTAMP", do_access_timestamp());
            client.DefaultRequestHeaders.Add("ACCESS-SIGN", do_access_sign(s));
            var endpoint = new Uri(url + s);
            var result = client.GetAsync(endpoint).Result;
            var jsonstring = result.Content.ReadAsStringAsync().Result;
            return jsonstring;
        }
        public string do_access_timestamp()
        {
            DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var TimeStampNow = (DateTimeOffset.Now - epoch).TotalSeconds;
            return (((int)TimeStampNow ).ToString());
        }
        public string do_access_sign(string s)
        {
            var param = new SortedDictionary<string, string>
                {
                    { "", s},
                };
            var str = String.Join("", param.Values);
            StringBuilder sb = new StringBuilder();
            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretkey));
            sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
            for (int i = 0; i < sha256.Hash.Length; i++)
            {
                sb.Append(sha256.Hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
        //JSON DATA
        public class Root
        {

            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("data")]
            public Data[] Data { get; set; }
        }
        public class Data
        {
            [JsonProperty("symbol")]
            public string Symbol { get; set; }
            [JsonProperty("order_id")]
            public string Order_id { get; set; }

            [JsonProperty("created_date")]
            public long Created_date { get; set; }

            [JsonProperty("finished_date")]
            public long Finished_date { get; set; }

            [JsonProperty("price")]
            public double Price { get; set; }

            [JsonProperty("amount")]
            public double Amount { get; set; }

            [JsonProperty("cash_amount")]
            public double Cash_amount { get; set; }

            [JsonProperty("executed_amount	")]
            public double Executed_amount { get; set; }

            [JsonProperty("avg_price")]
            public double Avg_price { get; set; }
            [JsonProperty("status")]
            public int status { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("kind")]
            public string Kind { get; set; }


        }


        //Глобальные переменные
        public string symbol { get; set; }
        public string secretkey { get; set; }
        public string key { get; set; }
        public string price { get; set; }
        public string amount { get; set; }



        //Покупка
        private void button1_Click_1(object sender, EventArgs e)
        {
            price = textBox1.Text;
            amount = textBox2.Text;

            string s = string.Format("symbol={0}&price={1}&amount={2}&type=buy", symbol, price, amount);
            string url = "https://openapi.digifinex.com/v3/spot/order/new?";
            do_postrequest(s, url);
        }


        //Продажа по той же price
        private void button2_Click(object sender, EventArgs e)
        {
            string s = string.Format("symbol={0}", symbol);
            string url = "https://openapi.digifinex.com/v3/spot/order/current?";
     
            //Json разбор get api запроса на активные ордера
            var json = JsonConvert.DeserializeObject<Root>(do_getrequest(s, url));
            // цикл по всем data, чтобы найти первое подходящее
            foreach (var value in json.Data)
            {
                //находим подходящий order_id по символу, цене и типу операции
                if (value.Symbol.ToLower() == this.symbol && value.Price.ToString() == this.price && value.Type == "buy")
                {
                    s = string.Format("order_id={0}", value.Order_id);
                    do_postrequest(s, "https://openapi.digifinex.com/v3/spot/order/cancel?");
                    break;
                }
            }    
        }
    }
}
