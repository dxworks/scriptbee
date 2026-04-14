import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ClientIdService {
  public readonly clientId: string;

  constructor() {
    this.clientId = crypto.randomUUID();
  }
}
