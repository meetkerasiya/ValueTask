using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace ValueTask
{
    class GitHubService
    {
        readonly IMemoryCache _chasedRepos = new MemoryCache(new MemoryCacheOptions());
        readonly HttpClient _httpClient = new HttpClient();

        public GitHubService()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ValueTask", "1.0"));

        }
        public async ValueTask<List<Repo>> GetReposAsync(string username)
        {
            string cachekey = username;
            List<Repo> repos = _chasedRepos.Get<List<Repo>>(cachekey);

            if (repos == null)
            {
                var result = await _httpClient.GetStringAsync($"http://api.github.com/users/{username}");
                repos = JsonSerializer.Deserialize<List<Repo>>(result);
                _chasedRepos.Set(cachekey, repos, TimeSpan.FromHours(1));
            }
            return repos;
        }
       public static void Main(string[] args)
        {
            var config = new ManualConfig()
        .WithOptions(ConfigOptions.DisableOptimizationsValidator)
        .AddValidator(JitOptimizationsValidator.DontFailOnError)
        .AddLogger(ConsoleLogger.Default)
        .AddColumnProvider(DefaultColumnProviders.Instance);

            var summary = BenchmarkRunner.Run<TaskBenchmarks>(config);

        }
    }
    public class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            AddJob(new Job());
            AddValidator(JitOptimizationsValidator.DontFailOnError);
            AddLogger(ConsoleLogger.Default);
            AddColumnProvider(DefaultColumnProviders.Instance);
        }
    }
}