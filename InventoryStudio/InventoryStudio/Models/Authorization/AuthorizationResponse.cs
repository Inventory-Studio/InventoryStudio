namespace InventoryStudio.Models.Authorization
{
    public class AuthorizationResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string Token { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
