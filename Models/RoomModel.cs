namespace SpliterX_API.Models
{
    public class RoomCreateRequest
    {
        public long AdminId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class RoomCreateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class RoomFetchAllResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; }
        public long AdminId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long TotalMembers { get; set; }
    }
    public class RoomUpdateRequest
    {
        public long RoomId { get; set; }
        public long AdminId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class RoomAddMemberRequest
    {
        public long UserId { get; set; }
        public long RoomId { get; set; }
    }
    public class RoomRemoveMemberRequest
    {
        public long UserId { get; set; }
        public long RoomId { get; set; }
    }
}
