namespace InventoryStudio.Models.AccessToken
{
    public class AccessTokenViewModel
    {
        public string AccessTokenID { get; set; }

        public string ApplicationName { get; set; }

        public string TokenName { get; set; }

        public string Token { get; set; }

        public string Secret { get; set; }

        public bool InActive { get; set; }

        public string Role { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
