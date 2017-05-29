namespace BusinessLayer.CodeClasses
{
    public class PlaceInFile
    {
        public int LineNumber { get; private set; }
        public string Spaces { get; private set; }

        public PlaceInFile(int lineNumber, string spaces)
        {
            LineNumber = lineNumber;
            Spaces = spaces;
        }
    }
}
