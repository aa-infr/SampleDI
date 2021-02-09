namespace ICT.Template.Core.Services.Models
{
    public class SampleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CreationDate { get; set; }

        public string UpdateDate { get; set; }

        public string Updator { get; set; }

        public string Creator { get; set; }

        public string NullableName { get; set; }

        public int NumberInt { get; set; }

        public double NumberDouble { get; set; }

        public int? NumberIntNullable { get; set; }

        public double? NumberDoubleNullable { get; set; }
    }
}