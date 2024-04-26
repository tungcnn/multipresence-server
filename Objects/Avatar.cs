namespace multipresence_backend.Objects
{
    public class Avatar
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }

        public Avatar(int playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }
    }
}
