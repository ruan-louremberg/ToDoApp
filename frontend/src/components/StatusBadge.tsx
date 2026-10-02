import type { TaskStatusDto } from "../types/task";

type StatusBadgeProps = {
  status: TaskStatusDto | null | undefined;
};

const STATUS_STYLES: Record<TaskStatusDto, { label: string; variant: "pending" | "in-progress" | "done" }> = {
  0: {
    label: "Pendente",
    variant: "pending",
  },
  1: {
    label: "Em progresso",
    variant: "in-progress",
  },
  2: {
    label: "Concluída",
    variant: "done",
  },
};

export function StatusBadge({ status }: StatusBadgeProps) {
  const config =
    status === null || status === undefined
      ? {
          label: "Desconhecido",
          variant: "pending" as const,
        }
      : STATUS_STYLES[status];

  return (
    <span className={`status-badge status-badge--${config.variant}`}>
      {config.label}
    </span>
  );
}