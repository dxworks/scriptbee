import { TestBed } from '@angular/core/testing';
import { InstanceAllocationService } from './instance-allocation.service';
import { InstanceService } from './instance.service';
import { ProjectStateService } from '../projects/project-state.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';
import { InstanceInfo } from '../../types/instance';
import { afterEach, beforeEach, describe, expect, it, Mock, vi } from 'vitest';

describe('InstanceAllocationService', () => {
  let service: InstanceAllocationService;
  let instanceServiceSpy: {
    getProjectInstances: Mock;
    allocateInstance: Mock;
    getInstanceStatus: Mock;
    deallocateInstance: Mock;
  };
  let projectStateServiceMock: {
    currentProjectId: ReturnType<typeof signal<string | null>>;
    currentInstanceId: ReturnType<typeof signal<string | null>>;
  };

  const mockInstances: InstanceInfo[] = [
    { id: 'inst-1', status: 'Running', creationDate: '2021-01-01' },
    { id: 'inst-2', status: 'Running', creationDate: '2021-01-02' },
  ];

  beforeEach(() => {
    instanceServiceSpy = {
      getProjectInstances: vi.fn(),
      allocateInstance: vi.fn(),
      getInstanceStatus: vi.fn(),
      deallocateInstance: vi.fn(),
    };

    projectStateServiceMock = {
      currentProjectId: signal<string | null>('test-project'),
      currentInstanceId: signal<string | null>(null),
    };

    TestBed.configureTestingModule({
      providers: [
        InstanceAllocationService,
        { provide: InstanceService, useValue: instanceServiceSpy },
        { provide: ProjectStateService, useValue: projectStateServiceMock },
      ],
    });

    service = TestBed.inject(InstanceAllocationService);
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('instancesResource', () => {
    it('should fetch instances for the current project', async () => {
      instanceServiceSpy.getProjectInstances.mockReturnValue(of(mockInstances));

      await waitForResourcesToResolve();

      expect(instanceServiceSpy.getProjectInstances).toHaveBeenCalledWith('test-project');
      expect(service.instancesResource.value()).toEqual(mockInstances);
    });

    it('should return empty list if no project ID', async () => {
      projectStateServiceMock.currentProjectId.set(null);

      await waitForResourcesToResolve();

      expect(service.instancesResource.value()).toEqual([]);
    });
  });

  describe('currentInstanceResource', () => {
    it('should find the current instance based on projectStateService', async () => {
      instanceServiceSpy.getProjectInstances.mockReturnValue(of(mockInstances));
      projectStateServiceMock.currentInstanceId.set('inst-2');

      await waitForResourcesToResolve();

      expect(service.currentInstanceResource.value()).toEqual(mockInstances[1]);
    });

    it('should return undefined if instance not found', async () => {
      instanceServiceSpy.getProjectInstances.mockReturnValue(of(mockInstances));
      projectStateServiceMock.currentInstanceId.set('non-existent');

      await waitForResourcesToResolve();

      expect(service.currentInstanceResource.value()).toBeUndefined();
    });
  });

  describe('allocateInstance', () => {
    it('should allocate and poll until not Allocating', async () => {
      const pollUrl = 'polling-url';
      instanceServiceSpy.allocateInstance.mockReturnValue(of(pollUrl));

      const allocatingInstance: InstanceInfo = { id: 'new-inst', status: 'Allocating', creationDate: 'now' };
      const runningInstance: InstanceInfo = { id: 'new-inst', status: 'Running', creationDate: 'now' };

      instanceServiceSpy.getInstanceStatus.mockReturnValueOnce(of(allocatingInstance)).mockReturnValueOnce(of(runningInstance));

      instanceServiceSpy.getProjectInstances.mockReturnValue(of([...mockInstances, runningInstance]));

      vi.useFakeTimers();

      let finalResult: InstanceInfo | undefined;
      service.allocateInstance('test-project').subscribe((finalInstance) => {
        finalResult = finalInstance;
      });

      await vi.advanceTimersByTimeAsync(1500);
      await vi.advanceTimersByTimeAsync(2000);

      expect(finalResult).toEqual(runningInstance);
      expect(instanceServiceSpy.allocateInstance).toHaveBeenCalledWith('test-project');
      expect(instanceServiceSpy.getInstanceStatus).toHaveBeenCalledTimes(2);
      expect(projectStateServiceMock.currentInstanceId()).toBe('new-inst');

      vi.useRealTimers();
    });
  });

  describe('deallocateInstance', () => {
    it('should deallocate, reload and clear current instance', async () => {
      instanceServiceSpy.deallocateInstance.mockReturnValue(of(void 0));
      projectStateServiceMock.currentInstanceId.set('inst-1');

      let called = false;
      service.deallocateInstance('test-project', 'inst-1').subscribe(() => {
        called = true;
      });

      expect(instanceServiceSpy.deallocateInstance).toHaveBeenCalledWith('test-project', 'inst-1');
      expect(projectStateServiceMock.currentInstanceId()).toBeNull();
      expect(called).toBe(true);
    });
  });

  describe('selection methods', () => {
    it('setCurrentInstance should update projectStateService', () => {
      service.setCurrentInstance(mockInstances[0]);

      expect(projectStateServiceMock.currentInstanceId()).toBe('inst-1');
    });

    it('clearCurrentInstance should set currentInstanceId to null', () => {
      projectStateServiceMock.currentInstanceId.set('inst-1');

      service.clearCurrentInstance();

      expect(projectStateServiceMock.currentInstanceId()).toBeNull();
    });
  });

  async function waitForResourcesToResolve() {
    await new Promise((resolve) => setTimeout(resolve, 0));
  }
});
