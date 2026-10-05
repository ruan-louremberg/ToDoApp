import type { TaskPriorityDto, TaskStatusDto } from "../types/task";

type BadgeVariant =
  | "pending"
  | "in-progress"
  | "done"
  | "low"
  | "medium"
  | "high"
  | "unknown";

type BadgeConfig = {
  label: string;
  variant: BadgeVariant;
};

type TaskBadgeProps =
  | {
      type: "status";
      value: TaskStatusDto | null | undefined;
    }
  | {
      type: "priority";
      value: TaskPriorityDto | null | undefined;
    };

const STATUS_STYLES: Record<TaskStatusDto, BadgeConfig> = {
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

const PRIORITY_STYLES: Record<TaskPriorityDto, BadgeConfig> = {
  0: {
    label: "Baixa",
    variant: "low",
  },
  1: {
    label: "Média",
    variant: "medium",
  },
  2: {
    label: "Alta",
    variant: "high",
  },
};

export function TaskBadge(props: TaskBadgeProps) {
  const config: BadgeConfig =
    props.value === null || props.value === undefined
      ? { label: "Desconhecido", variant: "unknown" }
      : props.type === "status"
        ? STATUS_STYLES[props.value]
        : PRIORITY_STYLES[props.value];

  return (
    <span className={`task-badge task-badge--${config.variant}`}>
      {config.label}
    </span>
  );
}