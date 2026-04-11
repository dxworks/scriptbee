import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InstanceManagerComponent } from './instance-manager.component';
import { InstanceAllocationService } from '../../services/instances/instance-allocation.service';
import { ProjectStateService } from '../../services/projects/project-state.service';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { signal, WritableSignal } from '@angular/core';
import { By } from '@angular/platform-browser';
import { InstanceInfo } from '../../types/instance';
import { afterEach, beforeEach, describe, expect, it, Mock, vi } from 'vitest';
import { OverlayContainer } from '@angular/cdk/overlay';

describe('InstanceManagerComponent', () => {
  let component: InstanceManagerComponent;
  let fixture: ComponentFixture<InstanceManagerComponent>;
  let instanceAllocationServiceSpy: {
    instancesResource: { value: WritableSignal<InstanceInfo[]>; reload: Mock };
    setCurrentInstance: Mock;
    allocateInstance: Mock;
    deallocateInstance: Mock;
    reload: Mock;
  };
  let projectStateServiceMock: {
    currentProjectId: WritableSignal<string | null>;
    currentInstanceId: WritableSignal<string | null>;
  };
  let dialogMock: { open: Mock };
  let overlayContainer: OverlayContainer;

  const mockInstances: InstanceInfo[] = [
    { id: 'inst-111111111111', status: 'Running', creationDate: '2021-01-01' },
    { id: 'inst-222222222222', status: 'Allocating', creationDate: '2021-01-02' },
  ];

  beforeEach(async () => {
    instanceAllocationServiceSpy = {
      instancesResource: {
        value: signal<InstanceInfo[]>(mockInstances),
        reload: vi.fn(),
      },
      setCurrentInstance: vi.fn(),
      allocateInstance: vi.fn().mockReturnValue(of({ id: 'new-inst', status: 'Running' })),
      deallocateInstance: vi.fn().mockReturnValue(of(void 0)),
      reload: vi.fn(),
    };

    projectStateServiceMock = {
      currentProjectId: signal<string | null>('test-project'),
      currentInstanceId: signal<string | null>(null),
    };

    dialogMock = {
      open: vi.fn().mockReturnValue({
        afterClosed: vi.fn().mockReturnValue(of(true)),
      }),
    };

    await TestBed.configureTestingModule({
      imports: [InstanceManagerComponent],
      providers: [
        { provide: InstanceAllocationService, useValue: instanceAllocationServiceSpy },
        { provide: ProjectStateService, useValue: projectStateServiceMock },
      ],
    })
      .overrideComponent(InstanceManagerComponent, {
        add: {
          providers: [{ provide: MatDialog, useValue: dialogMock }],
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(InstanceManagerComponent);
    component = fixture.componentInstance;
    overlayContainer = TestBed.inject(OverlayContainer);
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  async function openInstanceMenu() {
    fixture.detectChanges();
    const trigger = fixture.debugElement.query(By.css('.instance-button')).nativeElement;
    trigger.click();
    fixture.detectChanges();
    await fixture.whenStable();
  }

  function getOverlayContainerElement() {
    return overlayContainer.getContainerElement();
  }

  async function waitForSignalsAndEffects() {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 10));
  }

  it('should be created', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should display "No Instance" when no instance is selected', () => {
    projectStateServiceMock.currentInstanceId.set(null);
    fixture.detectChanges();

    const buttonText = fixture.debugElement.query(By.css('.instance-button-text')).nativeElement;
    expect(buttonText.textContent).toContain('No Instance');
  });

  it('should automatically select the first instance if none is selected and list is not empty', async () => {
    await waitForSignalsAndEffects();
    expect(instanceAllocationServiceSpy.setCurrentInstance).toHaveBeenCalledWith(mockInstances[0]);
  });

  it('should display the current instance ID in the button', () => {
    projectStateServiceMock.currentInstanceId.set('inst-111111111111');
    fixture.detectChanges();

    const buttonText = fixture.debugElement.query(By.css('.instance-button-text')).nativeElement;
    expect(buttonText.textContent).toContain('inst-111');
  });

  it('should list all instances in the menu', async () => {
    await openInstanceMenu();

    const container = getOverlayContainerElement();
    const menuItems = container.querySelectorAll('.instance-menu-item');
    expect(menuItems.length).toBe(2);
  });

  it('should call setCurrentInstance when an instance is selected from the menu', async () => {
    await openInstanceMenu();

    const container = getOverlayContainerElement();
    const selectButtons = container.querySelectorAll('.instance-select-action');
    (selectButtons[1] as HTMLElement).click();

    expect(instanceAllocationServiceSpy.setCurrentInstance).toHaveBeenCalledWith(mockInstances[1]);
  });

  it('should call allocateInstance when the allocate button is clicked', async () => {
    await openInstanceMenu();

    const container = getOverlayContainerElement();
    const menuButtons = container.querySelectorAll('button[mat-menu-item]');
    const allocateButton = Array.from(menuButtons).find((b) => b.textContent?.includes('Allocate New Instance')) as HTMLElement;
    allocateButton!.click();

    expect(instanceAllocationServiceSpy.allocateInstance).toHaveBeenCalledWith('test-project');
  });

  it('should open confirmation dialog and call deallocateInstance upon confirmation', async () => {
    fixture.detectChanges();
    await fixture.whenStable();

    const event = { stopPropagation: vi.fn() } as unknown as MouseEvent;
    component.onDeallocateInstance(event, mockInstances[0]);

    expect(dialogMock.open).toHaveBeenCalled();
    expect(instanceAllocationServiceSpy.deallocateInstance).toHaveBeenCalledWith('test-project', 'inst-111111111111');
  });

  it('should apply correct status icon and class', () => {
    expect(component.getStatusIcon('Running')).toBe('check_circle');
    expect(component.getStatusIcon('Allocating')).toBe('pending');
    expect(component.getStatusIcon('Deallocating')).toBe('delete_sweep');
    expect(component.getStatusIcon('NotFound')).toBe('error');

    expect(component.getStatusClass('Running')).toBe('running');
    expect(component.getStatusClass('Allocating')).toBe('allocating');
  });

  it('should reload instances when the refresh button is clicked', () => {
    fixture.detectChanges();
    const refreshButton = fixture.debugElement.query(By.css('.refresh-button')).nativeElement;
    refreshButton.click();

    expect(instanceAllocationServiceSpy.reload).toHaveBeenCalled();
  });
});
