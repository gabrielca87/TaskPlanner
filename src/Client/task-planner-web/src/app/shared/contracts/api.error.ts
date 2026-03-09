export type ApiErrorResponse = {
  statusCode: number;
  message: string;
  errors?: Record<string, string[]>;
};
