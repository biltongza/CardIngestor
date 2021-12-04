#nullable disable
public record IngestionConfig
{
    public string SearchPattern { get; set; }
    public IEnumerable<IngestDestination> Destinations { get; set; }
}