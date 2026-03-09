export type TaskItem = {
  id: string;
  userId: string;
  title: string;
  description: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
  createdBy: string;
  updatedBy: string;
};

export type CreateTaskItemRequest = {
  userId: string;
  title: string;
  description: string | null;
};

export type UpdateTaskItemRequest = {
  id: string;
  title: string;
  description: string | null;
};
