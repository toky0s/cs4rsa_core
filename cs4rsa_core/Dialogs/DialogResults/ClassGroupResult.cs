using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

namespace cs4rsa_core.Dialogs.DialogResults
{
    public class ClassGroupResult
    {
        public readonly ClassGroupModel ClassGroupModel;
        public readonly string SelectedRegisterCode;
        public readonly string SelectedSchoolClassName;

        public ClassGroupResult(
            ClassGroupModel classGroupModel, 
            string selectedRegisterCode, 
            string selectedSchoolClassName
        )
        {
            ClassGroupModel = classGroupModel;
            SelectedRegisterCode = selectedRegisterCode;
            SelectedSchoolClassName = selectedSchoolClassName;
        }
    }
}
