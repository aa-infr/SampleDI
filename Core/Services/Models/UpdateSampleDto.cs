namespace ICT.Template.Core.Services.Models
{
    public class UpdateSampleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string NullableName { get; set; }
        public int NumberInt { get; set; }
        public double NumberDouble { get; set; }
        public int? NumberIntNullable { get; set; }
        public double? NumberDoubleNullable { get; set; }
    }
}