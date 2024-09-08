namespace SpliterX_API.Models
{
    public class GroupCreateRequest
    {
        public long RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class GroupFetchAllResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long TotalTransactions { get; set; }
    }

}
