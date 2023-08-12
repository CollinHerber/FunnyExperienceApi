using Cowbot.Server.Lib;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Cowbot.Server.ThirdPartyServices.Youtube;

public class YoutubeClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerSettings _jsonSerializerSettings = Constants.Json.SnakeCaseSerializerSettings;

    public YoutubeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public void CreateSubscribeWebhook(string channelId) {
        try {
            var callBackUrl = "YOUR CALL BACK URL HERE";
            var topicUrl = $"https://www.youtube.com/xml/feeds/videos.xml?channel_id={channelId}";
            var subscribeUrl = "https://pubsubhubbub.appspot.com/subscribe";
            string postDataStr = $"hub.mode=subscribe&hub.verify_token={Guid.NewGuid().ToString()}&hub.verify=async&hub.callback={HttpUtility.UrlEncode(callBackUrl)}&hub.topic={HttpUtility.UrlEncode(topicUrl)}";
            byte[] postData = Encoding.UTF8.GetBytes(postDataStr);

            var request = (HttpWebRequest)WebRequest.Create(subscribeUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Flush();
            requestStream.Close();

            var webResponse = (HttpWebResponse)request.GetResponse();
            if (request.HaveResponse)
            {
                var responseStream = webResponse.GetResponseStream();
                var responseReader = new StreamReader(responseStream, Encoding.UTF8);
                responseReader.ReadToEnd();
            }
            else
            {
                throw new Exception("Didn't receive any response from the hub");
            }

        }
        catch (Exception ex){
            throw new Exception($"Error publishing Channel ID : {channelId}", ex);
        }
    }
    
    private void SetupHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
    }
}