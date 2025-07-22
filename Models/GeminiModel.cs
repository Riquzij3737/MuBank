namespace Mubank.Models
{
    public class GeminiModel
    {
        public Root raiz {  get; set; }
    }
   
    public class Root
    {
        public List<Content> Contents { get; set; }
    }

    public class Content
    {
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }


}
