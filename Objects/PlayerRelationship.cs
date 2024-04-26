namespace multipresence_backend.Objects
{
    public class PlayerRelationship
    {
        public int PlayerId { get; set; }
        public int FriendId { get; set; }

        public PlayerRelationship(int PlayerId, int FriendId) { this.PlayerId = PlayerId; this.FriendId = FriendId; }
    }
}
