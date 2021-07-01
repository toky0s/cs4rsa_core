using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho toàn bộ chương trình học.
    /// </summary>
    public class ProgramDiagram
    {
        private ProgramFolder _outlineRoot;
        private ProgramFolder _physicalEducationRoot;
        private ProgramFolder _industryOutlineRoot;
        private ProgramFolder _specializedRoot;

        public ProgramDiagram(ProgramFolder outlineRoot, ProgramFolder physicalEducationRoot,
                              ProgramFolder industryOutlineRoot, ProgramFolder specializedRoot)
        {
            _outlineRoot = outlineRoot;
            _physicalEducationRoot = physicalEducationRoot;
            _industryOutlineRoot = industryOutlineRoot;
            _specializedRoot = specializedRoot;
        }

        public List<ProgramSubject> GetAllProSubject()
        {
            List<ProgramSubject> programSubjects = new List<ProgramSubject>();
            programSubjects.AddRange(_outlineRoot.GetProgramSubjects());
            programSubjects.AddRange(_physicalEducationRoot.GetProgramSubjects());
            programSubjects.AddRange(_industryOutlineRoot.GetProgramSubjects());
            programSubjects.AddRange(_specializedRoot.GetProgramSubjects());
            return programSubjects;
        }
    }
}
