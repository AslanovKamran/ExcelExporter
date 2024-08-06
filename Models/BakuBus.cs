using System.ComponentModel.DataAnnotations;

namespace ExcelExportetWpf.Models
{
    public class BakuBus
    {
        [Key]
        public int IX { get; set; }

        public string? Plate { get; set; }
        public string? Tag { get; set; }
        public string? VehicleId { get; set; }
        public string? CardCode { get; set; }

        public BakuBus(string plate, string tag, string vehicleId, string cardCode)
        {
            Plate = plate;
            Tag = tag;
            VehicleId = vehicleId;
            CardCode = cardCode;
        }
        public override string ToString() => @$"{Plate} {Tag} {VehicleId} {CardCode}";

    }
}
