namespace api.Dtos
{
    public class UserCreateNotificationEvent
    {
        public string Receipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
