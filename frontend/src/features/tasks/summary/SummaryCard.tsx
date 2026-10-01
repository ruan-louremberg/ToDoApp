import styles from "./SummaryCard.module.css";

interface SummaryCardProps {
  title: string;
  value: number;
}

export function SummaryCard({
  title,
  value,
}: SummaryCardProps) {
  return (
    <div className={styles.card}>
      <span className={styles.title}>
        {title}
      </span>

      <strong className={styles.value}>
        {value}
      </strong>
    </div>
  );
}