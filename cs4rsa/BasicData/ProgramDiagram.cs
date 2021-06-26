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
    class ProgramDiagram
    {
        private List<ProgramFolder> _folderNodes;
        private List<ProgramSubject> _subjectNodes;
        private List<IProgramNode> _programNodes;

        public ProgramDiagram(List<ProgramFolder> folderNode, List<ProgramSubject> subjectNode)
        {
            _subjectNodes = subjectNode;
            _folderNodes = DivChildNode(folderNode, subjectNode);
            _programNodes.AddRange(_folderNodes);
            _programNodes.AddRange(_subjectNodes);
        }

        /// <summary>
        /// Copy các ProgramSubject con vào các ProgramFolder.
        /// </summary>
        /// <param name="nodes">Danh sách tất cả các node bao gồm cả subject và folder.</param>
        private List<ProgramFolder> DivChildNode(List<ProgramFolder> folderNodes, List<ProgramSubject> subjectNodes)
        {
            List<ProgramFolder> programFolders = new List<ProgramFolder>();
            foreach (ProgramFolder node in folderNodes)
            {
                List<ProgramSubject> subjects = GetChildProgramSubject(node, subjectNodes);
                node.AddNodes(subjects);
                programFolders.Add(node);
            }
            return programFolders;
        }

        private List<ProgramSubject> GetChildProgramSubject(ProgramFolder parrent, List<ProgramSubject> subjects)
        {
            List<ProgramSubject> childSubjects = new List<ProgramSubject>();
            foreach (ProgramSubject subject in subjects)
            {
                if (subject.GetChildOfNode() == parrent.GetIdNode())
                    childSubjects.Add(subject);
            }
            return childSubjects;
        }

        //public bool IsCompletedNode(IProgramNode node)
        //{
        //    if (node is ProgramSubject)
        //    {
        //        ProgramSubject subject = node as ProgramSubject;
        //        return subject.IsCompleted();
        //    }
        //    else
        //    {
        //        ProgramFolder folderNode = node as ProgramFolder;
        //        bool flag = true;
        //        List<IProgramNode> childNodes = GetChildNodes(node, _programNodes);
        //        foreach (IProgramNode n in childNodes)
        //        {
        //            if (n is ProgramSubject)
        //            {
        //                ProgramSubject subject = n as ProgramSubject;
        //                if (subject.IsUnLearn())
        //                    flag = flag && false;
        //            }
        //            else
        //            {
        //                if (IsCompletedNode(n))
        //                    flag = flag && false;
        //            }
        //        }
                
        //    }
        //}

        private List<IProgramNode> GetFolderNodesInCollection(List<IProgramNode> nodes)
        {
            List<IProgramNode> folderNodes = new List<IProgramNode>();
            foreach (IProgramNode node in nodes)
            {
                if (node is ProgramFolder)
                    folderNodes.Add(node);
            }
            return folderNodes;
        }


        /// <summary>
        /// Kiểm tra một danh sách các node xem có chứa một folder node nào đó không.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private bool IsHaveFolderNodeIn(List<IProgramNode> nodes)
        {
            foreach (IProgramNode node in nodes)
            {
                if (node is ProgramFolder)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Trả về một List chứa các child node của một node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<IProgramNode> GetChildNodes(IProgramNode parrent, List<IProgramNode> nodes)
        {
            List<IProgramNode> childNodes = new List<IProgramNode>();
            foreach(IProgramNode n in nodes)
            {
                if (n.GetChildOfNode() == parrent.GetIdNode())
                    childNodes.Add(n);
            }
            return childNodes;
        }
    }
}
