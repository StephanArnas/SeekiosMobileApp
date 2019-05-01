namespace SeekiosApp.Model
{
    public class SeekiosMarkerIdAssociation
    {
        public string SeekiosId { get; set; }

        public string MarkerId { get; set; }

        public SeekiosMarkerIdAssociation(string seekiosId, string markerId)
        {
            SeekiosId = seekiosId;
            MarkerId = markerId;
        }
    }
}
