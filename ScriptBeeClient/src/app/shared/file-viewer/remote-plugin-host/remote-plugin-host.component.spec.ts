import { describe, expect, it, vi, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { Component, input, Type } from '@angular/core';
import { RemotePluginHostComponent } from './remote-plugin-host.component';
import { loadRemoteModule } from '@angular-architects/native-federation';

vi.mock('@angular-architects/native-federation', () => ({
  loadRemoteModule: vi.fn(),
}));

@Component({
  selector: 'app-test-remote',
  template: '<div>Plugin Content Successfully Loaded!</div>',
  standalone: true,
})
class TestRemoteComponent {
  inputs = input<unknown>();
}

describe('RemotePluginHostComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RemotePluginHostComponent, TestRemoteComponent],
    }).compileComponents();

    vi.mocked(loadRemoteModule).mockReset();
  });

  it('should render the remote component content on success', async () => {
    let resolvePromise: (value: Record<string, Type<unknown>>) => void = () => {
      // Intentional empty block to handle assignment before promise creation
    };
    const promise = new Promise<Record<string, Type<unknown>>>((resolve) => {
      resolvePromise = resolve;
    });
    vi.mocked(loadRemoteModule).mockReturnValue(promise);

    const fixture = TestBed.createComponent(RemotePluginHostComponent);
    const plugin = {
      remoteName: 'test',
      exposedModule: './Component',
      componentName: 'App',
    };
    fixture.componentRef.setInput('pluginOutlet', plugin);

    fixture.detectChanges();

    resolvePromise({ App: TestRemoteComponent });
    await promise;

    fixture.detectChanges();
    await new Promise((resolve) => setTimeout(resolve));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Plugin Content Successfully Loaded!');
  });

  it('should display an error message if the plugin fails to load', async () => {
    let rejectPromise: (reason: Error) => void = () => {
      // Intentional empty block to handle assignment before promise creation
    };
    const promise = new Promise<Record<string, Type<unknown>>>((_, reject) => {
      rejectPromise = reject;
    });
    vi.mocked(loadRemoteModule).mockReturnValue(promise);

    const fixture = TestBed.createComponent(RemotePluginHostComponent);
    const plugin = {
      remoteName: 'test',
      exposedModule: './Component',
      componentName: 'App',
    };
    fixture.componentRef.setInput('pluginOutlet', plugin);

    fixture.detectChanges();

    rejectPromise(new Error('Network error'));
    await promise.catch(() => {
      // Intentional catch block suppression for testing rejection flow
    });

    fixture.detectChanges();
    await new Promise((resolve) => setTimeout(resolve));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Failed to load plugin');
  });
});
