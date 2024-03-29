﻿using System.Net;
using System.Text;

namespace WB_parser.Parsing
{
    public class GetCode
    {
        /// <summary>
        /// Получаем html код страницы сайта
        /// </summary>
        /// <param name="urlAddress"> url с которого берем информацию </param>
        /// <returns> возвращаем html код </returns>
        public static string GetHtmlCode(string urlAddress)
        {
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if(response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if(response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }

            return data;
        }
    }
}
