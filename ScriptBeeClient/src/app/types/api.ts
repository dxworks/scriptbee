export interface ErrorResponse {
  title: string;
  status: number;
  detail?: string;
  errors?: Record<string, string[]>;
}
