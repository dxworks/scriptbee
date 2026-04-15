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
  state: string;
}

const hubMocks = vi.hoisted(() => ({
  instance: {
    on: vi.fn(),
    start: vi.fn().mockResolvedValue(undefined),
    stop: vi.fn().mockResolvedValue(undefined),
    invoke: vi.fn().mockResolvedValue(undefined),
    state: 'Disconnected',
  } as unknown as MockHubConnection,
}));

vi.mock('@microsoft/signalr', () => {
  const builderMock = {
    withUrl: vi.fn().mockReturnThis(),
    withAutomaticReconnect: vi.fn().mockReturnThis(),
    build: vi.fn(() => hubMocks.instance),
  };

  return {
    HubConnectionBuilder: vi.fn().mockImplementation(function (this: unknown) {
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

  const getHandler = (methodName: string) => {
    const handler = mockHubConnection.on.mock.calls.find((c) => c[0] === methodName)?.[1];
    return handler as ((event: BaseScriptEvent) => void) | undefined;
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockHubConnection.state = 'Disconnected';

    TestBed.configureTestingModule({
      providers: [ProjectLiveUpdatesService, { provide: ClientIdService, useValue: { clientId: 'test-client-id' } }],
    });

    service = TestBed.inject(ProjectLiveUpdatesService);
  });

  it('should establish connection and register handlers on connect', async () => {
    await service.connect('proj-1');

    expect(mockHubConnection.start).toHaveBeenCalled();
    expect(mockHubConnection.on).toHaveBeenCalledWith('ScriptCreated', expect.any(Function));
  });

  it('should disconnect on disconnect()', async () => {
    mockHubConnection.state = 'Connected';
    await service.connect('proj-1');
    await service.disconnect();

    expect(mockHubConnection.stop).toHaveBeenCalled();
  });

  describe('Event Filtering', () => {
    it('should emit events from other clients', async () => {
      await service.connect('proj-1');
      const spy = vi.fn();
      service.scriptCreated$.subscribe(spy);

      const handler = getHandler('ScriptCreated');
      expect(handler).toBeDefined();

      const event: BaseScriptEvent = { scriptId: 's1', projectId: 'p1', clientId: 'other-id' };
      handler!(event);

      expect(spy).toHaveBeenCalledWith(event);
    });

    it('should ignore events from self', async () => {
      await service.connect('proj-1');
      const spy = vi.fn();
      service.scriptCreated$.subscribe(spy);

      const handler = getHandler('ScriptCreated');
      expect(handler).toBeDefined();

      handler!({ scriptId: 's1', projectId: 'p1', clientId: 'test-client-id' });

      expect(spy).not.toHaveBeenCalled();
    });
  });
});
