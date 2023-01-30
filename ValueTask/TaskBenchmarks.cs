using BenchmarkDotNet.Attributes;

namespace ValueTask
{
    [MemoryDiagnoser]
    public class TaskBenchmarks
    {
        GitHubService gitHubService=new GitHubService();

        [Benchmark]
        public async Task GetReposAsync()
        {
            string[] names = new string[] { "meetkerasiya", "himanshu634" };
            for(int i=0;i<10000;i++)
            {
                foreach(var name in names)
                {
                    var repos=await gitHubService.GetReposAsync(name);
                }
            }
        }
    }
}
