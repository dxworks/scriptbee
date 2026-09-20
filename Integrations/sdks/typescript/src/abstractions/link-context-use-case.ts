export interface LinkContextUseCase {
  link(linkerIds: string[]): Promise<void>;
}
