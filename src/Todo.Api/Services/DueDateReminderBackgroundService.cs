using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Application.UseCases;

namespace ToDoApp.Api.Services;

public sealed class DueDateReminderBackgroundService : BackgroundService
{
    private static readonly TimeSpan ReminderTime = new(8, 0, 0);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(15);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeProvider _timeProvider;
    private readonly TimeZoneInfo _timeZone;
    private readonly ILogger<DueDateReminderBackgroundService> _logger;

    public DueDateReminderBackgroundService(
        IServiceScopeFactory scopeFactory,
        TimeProvider timeProvider,
        TimeZoneInfo timeZone,
        ILogger<DueDateReminderBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _timeProvider = timeProvider;
        _timeZone = timeZone;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DateOnly? lastSuccessfulRun = null;
        DateTimeOffset? retryAt = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            var utcNow = _timeProvider.GetUtcNow();
            var localNow = TimeZoneInfo.ConvertTime(utcNow, _timeZone);
            var localDate = DateOnly.FromDateTime(localNow.DateTime);
            var scheduledForToday = localNow.TimeOfDay >= ReminderTime;

            if (scheduledForToday && lastSuccessfulRun != localDate)
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<SendDueDateRemindersUseCase>();
                    var emailsSent = await useCase.ExecuteAsync(
                        localDate.ToDateTime(TimeOnly.MinValue),
                        stoppingToken);

                    lastSuccessfulRun = localDate;
                    retryAt = null;
                    _logger.LogInformation(
                        "Daily due-date reminder run completed for {LocalDate}; {EmailsSent} digest emails sent.",
                        localDate,
                        emailsSent);
                    continue;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Daily due-date reminder run failed for {LocalDate}; retrying in {RetryMinutes} minutes.",
                        localDate,
                        RetryDelay.TotalMinutes);
                    retryAt = _timeProvider.GetUtcNow().Add(RetryDelay);
                }
            }

            var nextRun = GetNextRunUtc(localNow, _timeZone, ReminderTime);
            var nextCheck = retryAt is { } retry && retry < nextRun
                ? retry
                : nextRun;
            var delay = nextCheck - _timeProvider.GetUtcNow();
            if (delay <= TimeSpan.Zero)
            {
                continue;
            }

            await Task.Delay(delay, _timeProvider, stoppingToken);
        }
    }

    internal static DateTimeOffset GetNextRunUtc(
        DateTimeOffset localNow,
        TimeZoneInfo timeZone,
        TimeSpan reminderTime)
    {
        var localDateTime = TimeZoneInfo.ConvertTime(localNow, timeZone).DateTime;
        var candidate = DateTime.SpecifyKind(
            localDateTime.Date.Add(reminderTime),
            DateTimeKind.Unspecified);

        if (localDateTime.TimeOfDay >= reminderTime)
        {
            candidate = candidate.AddDays(1);
        }

        while (timeZone.IsInvalidTime(candidate))
        {
            candidate = candidate.AddMinutes(1);
        }

        return TimeZoneInfo.ConvertTimeToUtc(candidate, timeZone);
    }
}
