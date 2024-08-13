namespace Hyjynx.Racer.Models
{
    public class TrackData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TextureName { get; set; }

        public int TextureRows { get; set; }

        public int TextureColumns { get; set; }

        public TrackCell[,] Cells { get; set; }
    }
}
