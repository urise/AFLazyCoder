namespace BusinessLayer.CodeAnalyzer.Searching
{
    public class PlaceInfo
    {
        public string FileName { get; set; }
        public int LineNumber { get; set; }

        public PlaceInfo() { }

        public PlaceInfo(string fileName, int line)
        {
            FileName = fileName;
            LineNumber = line;
        }
    }
}
