namespace AppCalisto.Data.Entities
{
    public class ObservationImage
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public int ObservationId { get; set; }
        public Observation Observation { get; set; }
    }
}
