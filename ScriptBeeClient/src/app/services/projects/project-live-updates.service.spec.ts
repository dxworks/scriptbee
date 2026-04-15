import { TestBed } from '@angular/core/testing';
import { ProjectLiveUpdatesService } from './project-live-updates.service';
import { ClientIdService } from '../common/client-id.service';
import { beforeEach, describe, expect, it, vi, type MockInstance } from 'vitest';
import { BaseScriptEvent } from '../../types/live-updates';

interface MockHubConnection {
  on: MockInstance<(methodName: string, newMethod: (...args: unknown[]) => void) => void>;
  start: MockInstance<() => Promise<void>>;
  stop: MockInstance<() => Promise<void>>;
  invoke: MockInstance<(methodName: string, ...args: unknown[]) => Promise<unknown>>;
}

const hubMocks = vi.hoisted(() => {
  return {
    instance: {
      on: vi.fn(),
      start: vi.fn(),
      stop: vi.fn(),
      invoke: vi.fn(),
    } as MockHubConnection,
  };
});

vi.mock('@microsoft/signalr', () => {
  const builderMock = {
    withUrl: vi.fn().mockReturnThis(),
    withAutomaticReconnect: vi.fn().mockReturnThis(),
    build: vi.fn(() => hubMocks.instance),
  };

  return {
    HubConnectionBuilder: vi.fn().mockImplementation(function () {
      return builderMock;
    }),
    HubConnectionState: {
      Connected: 'Connected',
      Disconnected: 'Disconnected',
    },
  };
});

describe('ProjectLiveUpdatesService', () => {
  let service: ProjectLiveUpdatesService;
  const mockHubConnection = hubMocks.instance;

  beforeEach(() => {
    vi.clearAllMocks();

    mockHubConnection.on.mockImplementation(() => undefined);
    mockHubConnection.start.mockResolvedValue(undefined);
    mockHubConnection.stop.mockResolvedValue(undefined);
    mockHubConnection.invoke.mockResolvedValue(undefined);

    TestBed.configureTestingModule({
      providers: [ProjectLiveUpdatesService, { provide: ClientIdService, useValue: { clientId: 'test-client-id' } }],
    });

    service = TestBed.inject(ProjectLiveUpdatesService);
  });

  it('should establishing connection and register handlers on connect', async () => {
    await service.connect('proj-1');

    expect(mockHubConnection.start).toHaveBeenCalled();
    expect(mockHubConnection.on).toHaveBeenCalledWith('ScriptCreated', expect.any(Function));
    expect(mockHubConnection.on).toHaveBeenCalledWith('ScriptUpdated', expect.any(Function));
    expect(mockHubConnection.on).toHaveBeenCalledWith('ScriptDeleted', expect.any(Function));
  });

  it('should disconnect on disconnect()', async () => {
    await service.connect('proj-1');
    await service.disconnect();

    expect(mockHubConnection.stop).toHaveBeenCalled();
  });

  describe('Event Filtering', () => {
    it('should emit events from other clients', async () => {
      await service.connect('proj-1');
      const spy = vi.fn();
      service.scriptCreated$.subscribe(spy);

      const scriptCreatedHandler = mockHubConnection.on.mock.calls.find((c) => c[0] === 'ScriptCreated')?.[1];
      expect(scriptCreatedHandler).toBeDefined();

      const event: BaseScriptEvent = { scriptId: 's1', projectId: 'p1', clientId: 'other-client-id' };
      scriptCreatedHandler!(event);

      expect(spy).toHaveBeenCalledWith(event);
    });

    it('should ignore events from self', async () => {
      await service.connect('proj-1');
      const spy = vi.fn();
      service.scriptCreated$.subscribe(spy);

      const scriptCreatedHandler = mockHubConnection.on.mock.calls.find((c) => c[0] === 'ScriptCreated')?.[1];

      scriptCreatedHandler!({ scriptId: 's1', projectId: 'p1', clientId: 'test-client-id' });

      expect(spy).not.toHaveBeenCalled();
    });
  });
});
