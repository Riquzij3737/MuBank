using Mubank.Models;
using System.Text.Json;
using System.Text;
using GeminiSharp.Client;
using GeminiSharp.Models.Response;

namespace Mubank.Services
{
    public class GeminiService
    {        
        public async Task<string> SendHttpPost(string message, string apikey)
        {
            using (HttpClient client = new HttpClient())
            {                
                var GeminiClient = new GeminiClient(client, apikey);

                var reponse = await GeminiClient.GenerateContentAsync("gemini-2.0-flash", message);

                var responseContent = reponse.Candidates[0].Content.Parts;
                string responsestring = null;
                var sb = new StringBuilder(responsestring);

                foreach (var part in responseContent)
                {
                    sb.Append(part.Text);
                }
                
                return sb.ToString();
            }
        }
    }
}
