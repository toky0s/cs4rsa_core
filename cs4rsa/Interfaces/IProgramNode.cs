using cs4rsa.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Interfaces
{
    public interface IProgramNode
    {
        string GetIdNode();
        string GetChildOfNode();
        NodeType GetNodeType();
    }
}
