namespace SchoolMeals.Client.Helpers
{
    public class FormParams
    {
        public string Heading { get; set; }
        public bool Access { get; set; }
        public bool Loading { get; set; }
        public bool Disabled { get; set; }
        public bool Empty { get; set; }
        public bool Edit { get; set; }
        public bool Image { get; set; }
        public bool Upload { get; set; }

        public string File { get; set; }
        public byte[] FileBinary { get; set; }
        public string FileName { get; set; }

        public string Request { get; set; }
        public string Response { get; set; }
    }

    public class GenericList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
