import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SignalrClient, SignalrConnection } from 'ngx-signalr-websocket';
import { ReplaySubject } from 'rxjs';
import { WatchedFile } from './watchedFile';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class NotificationsService {
  private connection?: SignalrConnection;

  watchedFiles = new ReplaySubject<WatchedFile>();

  constructor(httpClient: HttpClient) {
    const client = SignalrClient.create(httpClient);

    client.connect('/api/fileWatcherHub').subscribe({
      next: (connection) => {
        console.log(connection);
        this.connection = connection;
        this.setupWatchedFiles(connection);
      },
      error: (err) => console.error('SignalR Connection Error: ', err),
    });
  }

  send() {
    this.connection?.invoke('SendFileWatch', {
      path: 'p',
      content: 'c',
    });
  }

  private setupWatchedFiles(connection: SignalrConnection) {
    connection
      .on<[WatchedFile]>('ReceiveFileWatch')
      .pipe(map((value) => value[0]))
      .subscribe((value) => {
        this.watchedFiles.next(value);
      });
  }
}
