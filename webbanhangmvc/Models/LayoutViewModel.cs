namespace webbanhangmvc.Models
{
    public class LayoutViewModel
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Body { get; set; }
        public string JsLinks { get; internal set; }
        public string CssLinks { get; internal set; }
    }
}
