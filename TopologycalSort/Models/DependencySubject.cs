namespace TopologycalSort.Models
{
    /// <summary>
    /// Môn phụ thuộc
    /// </summary>
    public struct DependencySubject
    {
        public int Id;
        public string SubjectCode;
        public string Name;
        public List<int> PreSubjectCodeIds;
    }
}
