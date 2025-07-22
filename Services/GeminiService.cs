using Mubank.Models;
using System.Text.Json;
using System.Text;

namespace Mubank.Services
{
    public class GeminiService
    {
        public async Task<string> SendHttpPost()
        {
            using (HttpClient client = new HttpClient())
            {
                string BodyContent = JsonSerializer.Serialize<GeminiModel>(new GeminiModel()
                {
                    raiz = new Root()
                    {
                        Contents = new List<Content>()
                        {
                            new Content()
                            {
                                Parts = new List<Part>()
                                {
                                    new Part() { Text = "Gere uma frase biblica para alegrar o dia de uma pessoa de no maximo 30 linhas" }
                                }
                            }
                        }
                    }
                });
                var body = new StringContent(BodyContent, Encoding.UTF8, "application/json");


                var Result = await client.PostAsync("https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=AIzaSyCWbTec8TIwOEFdWkq5FyNsLvRGM2PO58Y",body);

                return await Result.Content.ReadAsStringAsync();
            }
        }
    }
}
