using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho một thư mục lớn của chương trình học
    /// chứa các phương thức xác định một thư mục đã hoàn thành hay chưa.
    /// ** Đây là class quan trọng giúp thao tác với các Node **
    /// ** Muốn thao tác với một node phải thông qua class này **
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
    }
}
