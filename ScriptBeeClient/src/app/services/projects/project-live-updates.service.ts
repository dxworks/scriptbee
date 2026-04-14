import { inject, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { ClientIdService } from '../client-id.service';
import { ScriptCreateEvent, ScriptDeletedEvent, ScriptUpdatedEvent } from '../../types/live-updates';

@Injectable({
  providedIn: 'root',
})
export class ProjectLiveUpdatesService {
  private hubConnection: signalR.HubConnection | null = null;
  private currentProjectId: string | null = null;
  private clientIdService = inject(ClientIdService);

  public readonly scriptCreated$ = new Subject<ScriptCreateEvent>();
  public readonly scriptUpdated$ = new Subject<ScriptUpdatedEvent>();
  public readonly scriptDeleted$ = new Subject<ScriptDeletedEvent>();

  async connect(projectId: string) {
    if (this.currentProjectId === projectId && this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.hubConnection) {
      await this.disconnect();
    }

    this.currentProjectId = projectId;
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl('/api/projectLiveUpdates').withAutomaticReconnect().build();

    this.hubConnection.on('ScriptCreated', (event: ScriptCreateEvent) => {
      if (event.clientId !== this.clientIdService.clientId) {
        this.scriptCreated$.next(event);
      }
    });

    this.hubConnection.on('ScriptUpdated', (event: ScriptUpdatedEvent) => {
      if (event.clientId !== this.clientIdService.clientId) {
        this.scriptUpdated$.next(event);
      }
    });

    this.hubConnection.on('ScriptDeleted', (event: ScriptDeletedEvent) => {
      if (event.clientId !== this.clientIdService.clientId) {
        this.scriptDeleted$.next(event);
      }
    });

    try {
      await this.hubConnection.start();
      await this.joinChannel(projectId, 'scripts');
    } catch (err) {
      console.error('Error while starting live updates connection: ', err);
    }
  }

  async disconnect() {
    if (this.hubConnection && this.currentProjectId) {
      try {
        await this.leaveChannel(this.currentProjectId, 'scripts');
        await this.hubConnection.stop();
      } catch (err) {
        console.error('Error while stopping live updates connection: ', err);
      }
      this.hubConnection = null;
      this.currentProjectId = null;
    }
  }

  private async joinChannel(projectId: string, channelName: string) {
    if (this.hubConnection) {
      await this.hubConnection.invoke('JoinChannel', projectId, channelName);
    }
  }

  private async leaveChannel(projectId: string, channelName: string) {
    if (this.hubConnection) {
      await this.hubConnection.invoke('LeaveChannel', projectId, channelName);
    }
  }
}
