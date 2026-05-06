using BetterCRM.Core.Extensions;

namespace BetterCRM.Core.Models
{
    public class PayrollRecord : TenantEntity
    {
        public Guid UserId { get; internal set; }
        public DateTime PeriodStart { get; internal set; }
        public DateTime PeriodEnd { get; internal set; }

        public decimal ScheduledHours { get; internal set; }
        public decimal ActualHours { get; internal set; }

        // ✅ ИЗМЕНЕНО: один PenaltyHours → три поля
        public decimal AttendancePenaltyHours { get; internal set; }  // опоздания / ранний уход
        public decimal TicketPenaltyHours { get; internal set; }       // просроченные тикеты
        public decimal TotalPenaltyHours { get; internal set; }        // сумма двух штрафов

        // ✅ ИЗМЕНЕНО: BillableHours → FinalBillableHours (название точнее)
        public decimal FinalBillableHours { get; internal set; }       // ActualHours - TotalPenaltyHours
        public decimal HourlyRate { get; internal set; }
        public decimal CalculatedSalary { get; internal set; }

        // ✅ ИЗМЕНЕНО: string → enum
        public PayrollStatus Status { get; internal set; } = PayrollStatus.Calculated;

        public User User { get; internal set; } = null!;

        private PayrollRecord() { }

        public static (PayrollRecord? rec, string? error) Create(
            Guid organizationId, Guid userId,
            DateTime periodStart, DateTime periodEnd,
            decimal scheduled, decimal actual,
            decimal attendancePenalty, decimal ticketPenalty,
            decimal rate)
        {
            if (userId == Guid.Empty)
                return (null, "Некорректный пользователь");
            if (periodStart >= periodEnd)
                return (null, "Дата начала периода должна быть раньше даты окончания");
            if (rate <= 0)
                return (null, "Ставка должна быть больше нуля");

            var totalPenalty = Math.Round(attendancePenalty + ticketPenalty, 2);
            var finalBillable = Math.Round(Math.Max(0, actual - totalPenalty), 2);
            var salary = Math.Round(finalBillable * rate, 2);

            return (new PayrollRecord
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                PeriodStart = periodStart.Date,
                PeriodEnd = periodEnd.Date,
                ScheduledHours = Math.Round(scheduled, 2),
                ActualHours = Math.Round(actual, 2),
                AttendancePenaltyHours = Math.Round(attendancePenalty, 2),
                TicketPenaltyHours = Math.Round(ticketPenalty, 2),
                TotalPenaltyHours = totalPenalty,
                FinalBillableHours = finalBillable,
                HourlyRate = Math.Round(rate, 2),
                CalculatedSalary = salary,
                Status = PayrollStatus.Calculated,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        // ✅ ИЗМЕНЕНО: валидация через enum, защита от некорректных переходов
        public void ChangeStatus(PayrollStatus newStatus)
        {
            var validTransitions = new Dictionary<PayrollStatus, PayrollStatus[]>
            {
                [PayrollStatus.Calculated] = new[] { PayrollStatus.Approved },
                [PayrollStatus.Approved] = new[] { PayrollStatus.Paid, PayrollStatus.Calculated },
                [PayrollStatus.Paid] = Array.Empty<PayrollStatus>()
            };

            if (!validTransitions[Status].Contains(newStatus))
                throw new InvalidOperationException(
                    $"Нельзя перевести PayrollRecord из '{Status}' в '{newStatus}'");

            Status = newStatus;
            MarkAsUpdated();
        }

        // Перегрузка для совместимости с вызовами через строку (контроллеры, сервисы)
        public void ChangeStatus(string newStatus)
        {
            if (!Enum.TryParse<PayrollStatus>(newStatus, ignoreCase: true, out var parsed))
                throw new InvalidOperationException(
                    $"Недопустимый статус: '{newStatus}'. " +
                    $"Допустимые: {string.Join(", ", Enum.GetNames<PayrollStatus>())}");

            ChangeStatus(parsed);
        }

        public void Approve() => ChangeStatus(PayrollStatus.Approved);
        public void MarkAsPaid() => ChangeStatus(PayrollStatus.Paid);
    }
}