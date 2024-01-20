namespace InventoryStudio.Models.Authorization
{
    public class AuthorizationResponse
    {
        public string TokenId { get; set; }
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string Token { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
