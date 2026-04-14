import { randomUUID } from 'crypto';

export class ClientIdService {
  private static _clientId: string | undefined;

  public static get clientId(): string {
    if (!this._clientId) {
      this._clientId = randomUUID();
    }
    return this._clientId;
  }
}
