using System.Net;

namespace WB_parser.Parsing
{
    public class GetRequest
    {
        HttpWebRequest _request;
        string _address;
        
        public string Response { get; set; }

        public GetRequest(string address)
        {
            _address = address;
        }

        public void Run()
        {
            _request = (HttpWebRequest)HttpWebRequest.Create(_address);
            _request.Method = "GET";

            try
            {

                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                //Console.WriteLine(Response);

            }
            catch (Exception ex)
            {
                Response = ex.Message;
            }
        }
    }
}
