interface SummaryCardProps {
  title: string;
  value: number;
}

export function SummaryCard({
  title,
  value,
}: SummaryCardProps) {
  return (
    <div className="summary-card">
      <span className="summary-card__title">
        {title}
      </span>

      <strong className="summary-card__value">
        {value}
      </strong>
    </div>
  );
}