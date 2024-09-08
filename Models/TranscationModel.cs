namespace SpliterX_API.Models
{
    public class TransactionCreateRequest
    {
        public long GroupId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public short PaymentMode { get; set; }
        public long CreatedBy { get; set; }
        public long[] MemberIds { get; set; }
        public int[] Spent { get; set; }
        public int[] Expand { get; set; }
    }

    public class TransactionUpdateRequest
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public short PaymentMode { get; set; }
        public DateTime TransactionTime { get; set; }
        public long UpdatedBy { get; set; }
    }

    public class TransactionFetchAllByUserIdResponseModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string RoomName { get; set; }
        public string GroupName { get; set; }
        public int Amount { get; set; }
        public int Spent { get; set; }
        public int Expand { get; set; }
        public string PaymentMode { get; set; }
        public DateTime TransactionTime { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class TransactionFetchAllByGroupIdResponseModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string PaymentMode { get; set; }
        public DateTime TransactionTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }

}
