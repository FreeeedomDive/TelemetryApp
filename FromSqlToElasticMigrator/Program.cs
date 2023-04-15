// See https://aka.ms/new-console-template for more information

using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Logs.Repository;

var databaseContext = new DatabaseContext();

var logsSqlRepository = new SqlRepository<LogStorageElement>(databaseContext);
var logRepository = new LogRepository(logsSqlRepository);
var logs =  await logRepository.FindAsync(new LogFilterDto());

var elasticLogRepository = new ElasticLogRepository("https://localhost:9200", "<USERNAME>", "<PASSWORD>", "<INDEX>");
await elasticLogRepository.CreateIndexWithDynamicMapping("<INDEX>");
elasticLogRepository.BulkAllObservable(logs);