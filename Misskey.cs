using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MisskeyRemoteDeliver
{

    #region users/notes

    public class NotesRequest_Rootobject
    {
        public string userId { get; set; }
        public int limit { get; set; }
    }
    public class NotesRequest2_Rootobject
    {
        public string userId { get; set; }
        public int limit { get; set; }
        public string untilId { get; set; }
    }

    #endregion

    #region ap/show
    public class APShowRequest_Rootobject
    {
        public string i { get; set; }
        public string uri { get; set; }
    }
    #endregion
    internal class Misskey
    {
        static HttpClient Client = new HttpClient();

        /// <summary>
        /// APIトークン
        /// </summary>
        string AccessToken;

        /// <summary>
        /// 投稿先ホスト名
        /// </summary>
        string MissKeyHost;

        /// <summary>
        /// 設定の設定(?)
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="host"></param>
        public void Setting(string Token, string host)
        {
            AccessToken = Token;
            MissKeyHost = host;
        }

        /// <summary>
        /// Post送信(Json)
        /// </summary>
        /// <param name="API">エンドポイント</param>
        /// <param name="Json"></param>
        /// <returns></returns>
        public string Post(string API, string Json)
        {
            //   HttpResponseMessage Respone;
            string ResultBody;
            HttpStatusCode ResStatus = HttpStatusCode.NotFound;
            var Content = new StringContent(Json, Encoding.UTF8, @"application/json");
            try
            {
                var Respone = Client.PostAsync($"https://{MissKeyHost}/api/{API}", Content);

                ResultBody = Respone.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(ResultBody+"\r\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return ResultBody;
        }
    }

}
