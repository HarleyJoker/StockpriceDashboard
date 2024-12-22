namespace StockpriceDashboard.Models.DTO
{
    public class StockResponseDto
    {
        public MetaDataDto MetaData { get; set; }
        public Dictionary<string, DailyDataDto> TimeSeriesDaily { get; set; }
    }
}
