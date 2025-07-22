using Mubank.Models;
using System.Text.Json;
using System.Text;
using GeminiSharp.Client;
using GeminiSharp.Models.Response;


namespace Mubank.Services
{
    public class GeminiService
    {
        public async Task<string> SendHttpPost(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                var apikey = "AIzaSyCWbTec8TIwOEFdWkq5FyNsLvRGM2PO58Y";
                var GeminiClient = new GeminiClient(client, apikey);

                var reponse = await GeminiClient.GenerateContentAsync("gemini-2.0", message);

                reponse.
            }
        }
    }
}
