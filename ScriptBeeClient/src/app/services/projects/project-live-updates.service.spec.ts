import { TestBed } from '@angular/core/testing';
import { ProjectLiveUpdatesService } from './project-live-updates.service';
import { ClientIdService } from '../common/client-id.service';
import { expect, it, describe, vi, beforeEach, type MockInstance } from 'vitest';
import { BaseScriptEvent } from '../../types/live-updates';
import * as signalR from '@microsoft/signalr';

interface MockHubConnection {
  on: MockInstance<(methodName: string, newMethod: (event: BaseScriptEvent) => void) => void>;
  start: MockInstance<() => Promise<void>>;
  stop: MockInstance<() => Promise<void>>;
  state: string;
}

describe('ProjectLiveUpdatesService', () => {
  let service: ProjectLiveUpdatesService;
  let mockHubConnection: MockHubConnection;

  beforeEach(() => {
    mockHubConnection = {
      on: vi.fn(),
      start: vi.fn().mockResolvedValue(undefined),
      stop: vi.fn().mockResolvedValue(undefined),
      state: 'Disconnected',
    };

    vi.spyOn(signalR.HubConnectionBuilder.prototype, 'withUrl').mockReturnThis();
    vi.spyOn(signalR.HubConnectionBuilder.prototype, 'withAutomaticReconnect').mockReturnThis();
    vi.spyOn(signalR.HubConnectionBuilder.prototype, 'build').mockReturnValue(mockHubConnection as unknown as signalR.HubConnection);

    TestBed.configureTestingModule({
      providers: [ProjectLiveUpdatesService, { provide: ClientIdService, useValue: { clientId: 'test-client-id' } }],
    });

    service = TestBed.inject(ProjectLiveUpdatesService);

    const internalService = service as unknown as {
      currentProjectId: string | null;
      hubConnection: unknown | null;
    };
    internalService.currentProjectId = null;
    internalService.hubConnection = null;
  });

  const getHandler = (methodName: string): ((event: BaseScriptEvent) => void) | undefined => {
    const call = mockHubConnection.on.mock.calls.find((c) => c[0] === methodName);
    return call ? (call[1] as (event: BaseScriptEvent) => void) : undefined;
  };

  it('should establish connection and register handlers on connect', async () => {
    await service.connect('proj-1');

    expect(mockHubConnection.start).toHaveBeenCalled();
    expect(mockHubConnection.on).toHaveBeenCalledWith('ScriptCreated', expect.any(Function));
  });

  it('should disconnect on disconnect()', async () => {
    await service.connect('proj-1');

    const internalService = service as unknown as {
      hubConnection: MockHubConnection;
      currentProjectId: string | null;
    };

    internalService.hubConnection.state = 'Connected';

    await service.disconnect();

    expect(mockHubConnection.stop).toHaveBeenCalled();
    expect(internalService.hubConnection).toBeNull();
    expect(internalService.currentProjectId).toBeNull();
  });

  describe('Event Filtering', () => {
    it('should emit events from other clients', async () => {
      await service.connect('proj-1');
      const spy = vi.fn();
      service.scriptCreated$.subscribe(spy);

      const handler = getHandler('ScriptCreated');
      expect(handler).toBeDefined();

      const event: BaseScriptEvent = { scriptId: 's1', projectId: 'p1', clientId: 'other-id' };
      handler?.(event);

      expect(spy).toHaveBeenCalledWith(event);
    });

    it('should ignore events from self', async () => {
      await service.connect('proj-1');
      const spy = vi.fn();
      service.scriptCreated$.subscribe(spy);

      const handler = getHandler('ScriptCreated');
      expect(handler).toBeDefined();

      handler?.({ scriptId: 's1', projectId: 'p1', clientId: 'test-client-id' });

      expect(spy).not.toHaveBeenCalled();
    });
  });
});
