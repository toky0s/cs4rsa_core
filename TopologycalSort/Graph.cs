using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using TopologycalSort.Models;

namespace TopologycalSort
{
    public class Graph
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly List<DependencySubject> _dependencySubjects;
        private readonly IEnumerable<ProgramSubject> _programSubjects;
        private readonly int _vertices;
        private readonly List<List<int>> _edges;

        /// <summary>
        /// Hàm tạo
        /// </summary>
        /// <param name="programSubjects">Danh sách các Program Subject</param>
        /// <param name="unitOfWork">Unit of work</param>
        private Graph(
            IEnumerable<ProgramSubject> programSubjects,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _dependencySubjects = new List<DependencySubject>();
            _programSubjects = programSubjects;
            _vertices = programSubjects.Count();
            _edges = new List<List<int>>();


            for (int i = 0; i < _vertices; i++)
            {
                _edges.Add(new List<int>());
            }
        }

        /// <summary>
        /// Thêm cạnh
        /// </summary>
        /// <param name="v">ID môn học</param>
        /// <param name="w">ID môn tiên quyết</param>
        private void AddEdge(int v, int w)
        {
            _edges[v].Add(w);
        }

        /// <summary>
        /// Đệ quy thực hiện topological
        /// </summary>
        /// <param name="v">ID đỉnh</param>
        /// <param name="visited">Danh sách đỉnh đã thăm</param>
        /// <param name="stack">Stack</param>
        private void TopologicalSortUtil(int v, bool[] visited,
                                 Stack<int> stack)
        {
            // Đánh dấu thăm đỉnh
            visited[v] = true;
            foreach (var vertex in _edges[v])
            {
                if (!visited[vertex])
                {
                    TopologicalSortUtil(vertex, visited, stack);
                }
            }

            // Push current vertex to
            // stack which stores result
            stack.Push(v);
        }

        /// <summary>
        /// Lấy ra danh sách phụ thuộc
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> TopologicalSort()
        {
            Stack<int> stack = new();

            // Mark all the vertices as not visited
            var visited = new bool[_vertices];

            for (int i = 0; i < _vertices; i++)
            {
                if (visited[i] == false)
                {
                    TopologicalSortUtil(i, visited, stack);
                }
            }

            foreach (var vertex in stack)
            {
                yield return vertex;
            }
        }

        public static async Task<Graph> Build(
            IEnumerable<ProgramSubject> programSubjects,
            IUnitOfWork unitOfWork)
        {
            Graph graph = new(programSubjects, unitOfWork);


            await graph.ParseDependencySubjects();

            #region Add Edges
            foreach (ProgramSubject programSubject in programSubjects)
            {
                DependencySubject dependencySubject = await graph.GetDependencySubject(programSubject);
                foreach (DependencySubject item in graph.GetEdgesOfNode(dependencySubject))
                {
                    graph.AddEdge(dependencySubject.Id, item.Id);
                }
            }
            #endregion
            return graph;
        }

        #region Xử lý Subject
        /// <summary>
        /// Lấy ra danh sách các DependencySubject từ ProgramSubject
        /// </summary>
        /// <returns></returns>
        private async Task ParseDependencySubjects()
        {
            foreach (ProgramSubject programSubject in _programSubjects)
            {
                DependencySubject dependencySubject = await GetDependencySubject(programSubject);

                // Chuyển các môn phụ thuộc thành Dependency Subject
                if (dependencySubject.PreSubjectCodeIds.Count > 0)
                {
                    DependencySubject dependencySubjectPre = await GetDependencySubject(dependencySubject.SubjectCode);
                    // Thêm vào graph nếu hắn chưa tồn tại
                    if (!_dependencySubjects.Any(ps => ps.SubjectCode.Equals(dependencySubjectPre.SubjectCode)))
                    {
                        _dependencySubjects.Add(dependencySubjectPre);
                    }
                }
                _dependencySubjects.Add(dependencySubject);
            }
        }

        private async Task<DependencySubject> GetDependencySubject(ProgramSubject programSubject)
        {
            DependencySubject dependencySubject = new();
            dependencySubject.SubjectCode = programSubject.SubjectCode;
            dependencySubject.Name = programSubject.Name;
            List<PreProDetail> preProDetails = await _unitOfWork
                .PreProDetails
                .GetPreProSubjectsByProgramSubjectId(programSubject.CourseId);
            List<int> subjectCodeIds = preProDetails
                .Select(preProDetail => _unitOfWork.PreParSubjects.GetById(preProDetail.PreParSubjectId).PreParSubjectId)
                .ToList();
            dependencySubject.PreSubjectCodeIds = subjectCodeIds;
            return dependencySubject;
        }

        /// <summary>
        /// Lấy ra Dependency Subject dựa theo mã môn.
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns></returns>
        private async Task<DependencySubject> GetDependencySubject(string subjectCode)
        {
            ProgramSubject programSubject = await _unitOfWork.ProgramSubjects.GetBySubjectCode(subjectCode);
            DependencySubject dependencySubject = await GetDependencySubject(programSubject);
            return dependencySubject;
        }

        /// <summary>
        /// Lấy ra danh sách các Edge của một node.
        /// </summary>
        /// <param name="dependencySubject">DependencySubject</param>
        /// <returns></returns>
        public IEnumerable<DependencySubject> GetEdgesOfNode(DependencySubject dependencySubject)
        {
            return _dependencySubjects.Where(ps => dependencySubject.PreSubjectCodeIds.Contains(ps.Id));
        }
        #endregion
    }
}
